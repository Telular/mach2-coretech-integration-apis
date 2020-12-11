using System;
using System.Collections;
using System.Threading.Tasks;
using CommPkt;
using System.Drawing;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;

namespace TankUtilityInterface
{
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;   
    using System.Text;
    

    public class TankUtilityTopicSubReader
    {
        private readonly string _topicName;
        private readonly string _subscriptionName;        
        
        // Prefetch should be larger than the max messages - more efficient.
        private readonly int _prefetchCount = 20;
        private readonly int _maxMessagesPerRead = 40;
        private MessageReceiver _messageReceiver;

        public Queue qReceive;
        public System.Threading.AutoResetEvent eventNewRxWork;
        public ListView listViewMsg;
        public ListView listViewStatus;

        public TankUtilityTopicSubReader(string subscriptionName, ListView listViewMsg, ListView lilistViewStatus) 
        {
            _topicName = Properties.Settings.Default.TopicName;
            _subscriptionName = subscriptionName;

            // Create receive queue/worker for MO messages received from NPhaseOne
            qReceive = Queue.Synchronized(new Queue());
            eventNewRxWork = new System.Threading.AutoResetEvent(false);

            this.listViewMsg = listViewMsg;
            this.listViewStatus = lilistViewStatus;

            InitializeServiceBus();
        }

           
        public TankUtilityTopicSubReader(string topicName, string subscriptionName)
        {
            _topicName = topicName;
            _subscriptionName = subscriptionName;

            // Create receive queue/worker for MO messages received from NPhaseOne
            qReceive = Queue.Synchronized(new Queue());
            eventNewRxWork = new System.Threading.AutoResetEvent(false);

            InitializeServiceBus();
        }

        private void InitializeServiceBus()
        {
            var connectionString = Properties.Settings.Default.TopicConnection;         

            RetryExponential retryPolicy = new RetryExponential(minimumBackoff: TimeSpan.FromSeconds(0), maximumBackoff: TimeSpan.FromSeconds(30), maximumRetryCount: 5);

            var entityPath = $"{_topicName}/Subscriptions/{_subscriptionName}";

            _messageReceiver = new MessageReceiver(connectionString, entityPath, ReceiveMode.PeekLock, retryPolicy, _prefetchCount);
                                
        }

        public async Task ReceiveMessagesLoopAsync()
        {          
            while (true)
            {
                var messageList = await _messageReceiver.ReceiveAsync(_maxMessagesPerRead, TimeSpan.FromSeconds(5));
                if (messageList == null) break;
                                                       
                foreach (var message in messageList)
                {
                    try
                    {                   
                        var aPayload = JsonConvert.DeserializeObject<Payload>(Encoding.UTF8.GetString(message.Body));
                        // Always log the raw message
                        Log.ErrorToFile(Properties.Settings.Default.LogFilePath + "\\TUI_DebugLog", Encoding.UTF8.GetString(message.Body));
                        // Need the "reading time" to add the message to the display                       
                        DateTime.TryParse(aPayload.Data["time"].ToString(), out DateTime dataTime);
                        // Display message
                        AddListViewMsgItem("Rx", dataTime, aPayload.TankId, Encoding.UTF8.GetString(message.Body), Color.Black);
                        
                        PrintPayloads(aPayload);
                        await _messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
                    }
                    catch(Exception ex)
                    {
                        //MKR What is message.MessageId (SBuxEx available)?  Should this have message.Body?
                        Log.ErrorToFile(Properties.Settings.Default.LogFilePath, string.Format("TankUtilityInterface : MessageId: {0}, {1}" , message.MessageId, ex.ToString()));
                        await _messageReceiver.DeadLetterAsync(message.SystemProperties.LockToken);                       
                    }
                }
             
            }
        }


        public void PrintPayloads(Payload payload)
        {          
            lock (Console.Out)
            {
                Dictionary<string, object> tankdata = payload.Data;

                if (tankdata.ContainsKey("telemetry"))
                {
                    Dictionary<string, object> telemetry;
                    telemetry = JsonConvert.DeserializeObject<Dictionary<string, object>>(tankdata["telemetry"].ToString());

                    if (telemetry?.Count == 0) return;

                    if ((String.IsNullOrWhiteSpace(payload.TankId))
                        && (tankdata.ContainsKey("time"))
                        && (tankdata.ContainsKey("percent"))
                        && (telemetry.ContainsKey("rsrp"))
                        && (telemetry.ContainsKey("rsrq"))
                        )
                    {
                        // This message has the required elements to be passed to CPM
                        if (DateTime.TryParse(tankdata["time"].ToString(), out DateTime tankdataTime))
                        {
                            //This message has the required values to be passed on to CPM
                            CommPkt.CPMessage cpMsg = new CPMessage();
                            StringBuilder sData = new StringBuilder();

                            sData.Append("[");
                            sData.Append("PC,");
                            sData.Append(tankdata["percent"]);

                            // Create message object for queueing          
                            cpMsg.dtUTCTimeStamp = tankdataTime;
                            cpMsg.iSourceType = (int)CommPkt.Constants.SourceType.TUI;
                            cpMsg.iDirectionType = (int)CommPkt.Constants.DirectionType.Rx;
                            cpMsg.sSourceNumber = payload.TankId;
                            cpMsg.iTranType = (int)CommPkt.Constants.TransType.Data;
                            cpMsg.iSequence = 0;
                            cpMsg.iDataLen = sData.Length;
                            cpMsg.iErrorCode = 0;
                            cpMsg.iNetworkErrCode = 0;
                            cpMsg.sData = sData.ToString();
                            cpMsg.iSMPPChannelID = 0;

                            try
                            {
                                DeliverMessage(cpMsg);
                            }
                            catch (Exception ex)
                            {
                                AddlistViewStatusItem("PrintPayloads, DeliverMessage Exception!: " + ex.Message, Color.Red);
                            }
                        }

                    }
                }
            }

        }

        #region Header Comments
        //*********************************************************************************************
        // AddListViewMsgItem
        //
        // PURPOSE:   This is a thread safe method to add a line to the message list view. 
        // 
        // USES:      
        // 
        //*********************************************************************************************
        #endregion

        private delegate void AddListViewMsgItemDelegate(string sDirection, DateTime dtTimeStamp, string sDevice, string sData, Color cText);

        public void AddListViewMsgItem(string sDirection, DateTime dtTimeStamp, string sDevice, string sData, Color cText)
        {
            // If current thread is not the same as controls thread then invoke a delegate call
            // Else just do it
            if (this.listViewMsg.InvokeRequired == true)
            {
                this.listViewMsg.Invoke(new AddListViewMsgItemDelegate(AddListViewMsgItem), new object[] { sDirection, dtTimeStamp, sDevice, sData, cText });
            }
            else
            {
                // If last item in list is visible then set autoscroll to true
                bool bAutoScroll = (this.listViewMsg.Items.Count > 0
                    && (this.listViewMsg.Height - this.listViewMsg.TopItem.Position.Y)
                        > this.listViewMsg.Items[this.listViewMsg.Items.Count - 1].Position.Y);

                ListViewItem item = new ListViewItem(DateTime.Now.ToString());
                item.ForeColor = cText;
                item.SubItems.Add(sDirection);
                item.SubItems.Add(dtTimeStamp.ToString());
                item.SubItems.Add(sDevice);
                item.SubItems.Add(sData);
                if (this.listViewMsg.Items.Count > 2000)
                {
                    this.listViewMsg.Items.RemoveAt(0);
                }
                this.listViewMsg.Items.Insert(this.listViewMsg.Items.Count, item);

                // Make sure new item is visible if previous last item was visible
                if (bAutoScroll)
                {
                    this.listViewMsg.Items[this.listViewMsg.Items.Count - 1].EnsureVisible();
                }

                // If Error message then copy it to error log file
                if (Color.Red == cText)
                {
                    Log.ErrorToFile(Properties.Settings.Default.LogFilePath, "TankUtilityInterface :" + sDirection + ":" + sDevice + ": 0 :" + sData);
                }
            }
        }
        #region Header Comments
        //*********************************************************************************************
        // AddlistViewStatusItem
        //
        // PURPOSE:   This is a thread safe method to add a line to the status list view. 
        // 
        // USES:      
        // 
        //*********************************************************************************************
        #endregion

        private delegate void AddlistViewStatusItemDelegate(string sStatus, Color cText);

        public void AddlistViewStatusItem(string sStatus, Color cText)
        {
            // If current thread is not the same as controls thread then invoke a delegate call
            // Else just do it
            if (this.listViewStatus.InvokeRequired == true)
            {
                this.listViewStatus.Invoke(new AddlistViewStatusItemDelegate(AddlistViewStatusItem), new object[] { sStatus, cText });
            }
            else
            {
                // If last item in list is visible then set autoscroll to true
                bool bAutoScroll = (listViewStatus.Items.Count > 0
                    && (listViewStatus.Height - listViewStatus.TopItem.Position.Y)
                    > listViewStatus.Items[listViewStatus.Items.Count - 1].Position.Y);

                ListViewItem item = new ListViewItem(DateTime.Now.ToString());
                item.ForeColor = cText;
                item.SubItems.Add(sStatus);
                if (listViewStatus.Items.Count > 2000)
                {
                    listViewStatus.Items.RemoveAt(0);
                }
                listViewStatus.Items.Insert(listViewStatus.Items.Count, item);

                // Make sure new item is visible if previous last item was visible
                if (bAutoScroll)
                {
                    listViewStatus.Items[listViewStatus.Items.Count - 1].EnsureVisible();
                }

                // If Error message then copy it to error log file
                if (Color.Red == cText)
                {
                    Log.ErrorToFile(Properties.Settings.Default.LogFilePath, "TankUtilityInterface :" + sStatus);
                }

            }
        }

        public async Task CloseMessagePumpAsync()
        {
            try
            {
                // close our communication with the MT queues and topics
                if (_messageReceiver != null && !_messageReceiver.IsClosedOrClosing)
                {
                    await _messageReceiver.CloseAsync();
                }          
            }
            catch (Exception ex)
            {
                Log.ErrorToFile(Properties.Settings.Default.LogFilePath + "\\TUI_DebugLog", string.Format(ex.Message));
            }
        }

        #region Header Comments
        //*********************************************************************************************
        // DeliverMessage()
        //
        // PURPOSE:   Sends received MO message to CommPktMgr via a TCP channel.
        // 
        // INPUTS:    Message:    Text of message
        //
        // OUTPUTS:   None
        // 
        // USES:      CommPkt DLL:  Defines Remoting object and MessageData object.
        // 
        //*********************************************************************************************
        #endregion
        public void DeliverMessage(CommPkt.CPMessage cpMsg)
        {
            try
            {
                // If Comm Packet Manager in not online todo:
                string sCommPktMgrURL = Properties.Settings.Default.RemotingRxURL;
                if (!CommPkt.Log.IsServerOnline(sCommPktMgrURL, Convert.ToInt32(Properties.Settings.Default.RemotingRxOnlineTimeout)))
                {
                    throw new ArgumentException("Unreachable Remoting PC:" + sCommPktMgrURL);
                }

                // Create a client TCP channel for the remoting call to the Comm Packet Manager.
                // If this fails don't worry it just means a channel is already registered based on
                // previous calls.

                // Set up the server channel.
                System.Collections.IDictionary dict = new System.Collections.Hashtable();
                dict.Add("timeout", Properties.Settings.Default.RemotingRXReplyTimeout);
                TcpChannel chan = new TcpChannel(dict, null, null);
                try
                {
                    ChannelServices.RegisterChannel(chan, false);   // NO SECURITY ENABLED
                }
                catch
                {
                    // Don't Log exception since we ignore register errors
                }

                // Create an instance of the remote object and give it the current received message.
                CommPkt.CPInterface remoteObject = (CommPkt.CPInterface)Activator.GetObject(typeof(CommPkt.CPInterface), sCommPktMgrURL);

                // Call remote method if object was created
                if (remoteObject.Equals(null))
                {
                    throw new ArgumentException("Unable to create remoting object:" + sCommPktMgrURL);
                }
                remoteObject.ReceivedPacket(cpMsg);
            }
            catch (Exception ex)
            {
                // Add packet to CommPktMgr recovery file since we can't remote it
                CommPkt.Log.RecoverMessageToFile(Properties.Settings.Default.LogFilePath, cpMsg);
                AddlistViewStatusItem("DeliverMessage Error! " + ex.Message + ", Remoting Failed", Color.Red);
            }
            return;
        }

    }

}
 
