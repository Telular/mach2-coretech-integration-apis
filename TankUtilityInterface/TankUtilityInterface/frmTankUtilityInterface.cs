using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using CommPkt;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using Microsoft.ServiceBus.Messaging;
using System.IO;

//using Microsoft.ServiceBus.Messaging;

namespace TankUtilityInterface
{
    
    #region Header Comments
    //*********************************************************************************************
    // FrmAzureInterface Class
    //
    // PURPOSE:   The Form for NPhaseOne Interface application. The class is derived from 
    //            CommPkt.IObserver so it can implement the TCP Channel server side method
    //            SrvReceivedPacket().
    // 
    // USES:      TankData DLL:  Defines the DB Table adpater, SQL connection string and Constants.
    //            CommPkt DLL:  Defines the CommPkt MessageData object, remoting object,
    //                          Loging methods.
    //            App.config:   "RemotingTxPort", "RemotingTxService", ...
    // 
    //*********************************************************************************************
    #endregion
    public partial class frmTankUtilityInterface : Form, CommPkt.IObserver
    {
        public frmTankUtilityInterface()
        {
            InitializeComponent();
            // Display version in title bar
            this.Text = this.Text
                        + " " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
                        + " " + Properties.Settings.Default.ApplicationType;

            // Add line to log file that application has started
            Log.ErrorToFile(Properties.Settings.Default.LogFilePath, "TankUtilityInterface.exe: Application Start Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            // Enable Test Button if in debug
#if DEBUG
            btnTest.Visible = true;
#else
            btnTest.Visible = false;
#endif
            // Create a server TCP channel for the remoting call from message sources.
            // Method SvrReceivedPacket() is the actual callback.
            // The Port number and Service name are saved in the tankutili.exe.config file,
            // see Settings.settings "RemotingTxPort" and "RemotingTxService".
            // The remoting object definition is in the CommPkt class library (dll).
            TcpChannel channel = new TcpChannel(Convert.ToInt32(Properties.Settings.Default.RemotingTxPort)); //todo:
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(CPInterface), Properties.Settings.Default.RemotingTxService, WellKnownObjectMode.Singleton);
            CommPkt.Cache.Attach(this);

            // Create transmit queue/worker for MT messages to send to NPhaseOne
            /*qTransmit = Queue.Synchronized(new Queue());
            eventNewTxWork = new System.Threading.AutoResetEvent(false);
            bkgWorkerTx.RunWorkerAsync();
            */
            // Create receive queue/worker for MO messages received from NPhaseOne
            qReceive = Queue.Synchronized(new Queue());
            eventNewRxWork = new System.Threading.AutoResetEvent(false);
            bkgWorkerRx.RunWorkerAsync();

        }
        #region Public Vars
        //*********************************************************************************************
        // Start of Public Vars.
        //*********************************************************************************************

        // Interface Vars
        public SubscriptionClient mobileOriginateSubClient;
        public HttpClient mobileTerminateHttpClient;

        public Queue qTransmit;
        public System.Threading.AutoResetEvent eventNewTxWork;

        public Queue qReceive;
        public System.Threading.AutoResetEvent eventNewRxWork;
      
        //*********************************************************************************************
        // End of Public Vars.
        //*********************************************************************************************
        #endregion

        #region Header Comments
        //*********************************************************************************************
        // SrvReceivedPacket
        //
        // PURPOSE:   The IObserver Member SrvReceivedPacket method is called by the remoting service
        //            when a MT message is sent to the TankUtilityInterface for transmission. 
        // 
        // USES:      qTransmit:
        //            eventNewTxWork:
        // 
        //*********************************************************************************************
        #endregion
        void CommPkt.IObserver.SrvReceivedPacket(CPMessage oCPMsg)
        {
            CPMessage cpMsg;
            cpMsg = new CPMessage(oCPMsg);

            // Queue message and set new work event
            qTransmit.Enqueue(cpMsg);
            eventNewTxWork.Set();
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

        private void AddListViewMsgItem(string sDirection, DateTime dtTimeStamp, string sDevice, string sData, Color cText)
        {
            // If current thread is not the same as controls thread then invoke a delegate call
            // Else just do it
            if (listViewMsg.InvokeRequired == true)
            {
                listViewMsg.Invoke(new AddListViewMsgItemDelegate(AddListViewMsgItem), new object[] { sDirection, dtTimeStamp, sDevice, sData, cText });
            }
            else
            {
                // If last item in list is visible then set autoscroll to true
                bool bAutoScroll = (listViewMsg.Items.Count > 0
                    && (listViewMsg.Height - listViewMsg.TopItem.Position.Y)
                        > listViewMsg.Items[listViewMsg.Items.Count - 1].Position.Y);

                ListViewItem item = new ListViewItem(DateTime.Now.ToString());
                item.ForeColor = cText;
                item.SubItems.Add(sDirection);
                item.SubItems.Add(dtTimeStamp.ToString());
                item.SubItems.Add(sDevice);
                item.SubItems.Add(sData);
                if (listViewMsg.Items.Count > 2000)
                {
                    listViewMsg.Items.RemoveAt(0);
                }
                listViewMsg.Items.Insert(listViewMsg.Items.Count, item);

                // Make sure new item is visible if previous last item was visible
                if (bAutoScroll)
                {
                    listViewMsg.Items[listViewMsg.Items.Count - 1].EnsureVisible();
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

        private void AddlistViewStatusItem(string sStatus, Color cText)
        {
            // If current thread is not the same as controls thread then invoke a delegate call
            // Else just do it
            if (listViewStatus.InvokeRequired == true)
            {
                listViewStatus.Invoke(new AddlistViewStatusItemDelegate(AddlistViewStatusItem), new object[] { sStatus, cText });
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

        #region Header Comments
        //*********************************************************************************************
        // listView_KeyDown
        //
        // PURPOSE:   Handles the key down event for all list boxs.
        //            If Ctrl-C is pressed then it copies selected items to clipboard. 
        // 
        // USES:      
        // 
        //*********************************************************************************************
        #endregion
        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            // Get the listview sender
            ListView lvSender = sender as ListView;
            if (null == lvSender)
            {
                return;
            }
            // If user pressed Ctrl-C then copy selected items to clipboard
            if (e.Control && e.KeyCode == Keys.C)
            {
                String sData = "";
                ListView.SelectedListViewItemCollection items = lvSender.SelectedItems;
                if (0 == items.Count)
                {
                    return;
                }

                Clipboard.Clear();
                foreach (ListViewItem item in items)
                {
                    sData += item.Text + " " + item.SubItems[1].Text;
                    if (5 == item.SubItems.Count)
                    {
                        sData += " " + item.SubItems[2].Text + " " + item.SubItems[3].Text + " " + item.SubItems[4].Text + Environment.NewLine;
                    }
                    else
                    {
                        sData += Environment.NewLine;
                    }
                }
                Clipboard.SetDataObject(sData);
            }
        }

        #region Header Comments
        //*********************************************************************************************
        // AddDebugLogItem
        //
        // PURPOSE:   This is a thread safe method to add a degug line to a file. 
        // 
        // USES:      
        // 
        //*********************************************************************************************
        #endregion

        private delegate void AddDebugLogItemDelegate(string sStatus);

        private void AddDebugLogItem(string sStatus)
        {
#if DEBUG
            // If current thread is not the same as controls thread then invoke a delegate call
            // Else just do it
            if (listViewStatus.InvokeRequired == true)
            {
                listViewStatus.Invoke(new AddDebugLogItemDelegate(AddDebugLogItem), new object[] { sStatus });
            }
            else
            {
                Log.ErrorToFile(Properties.Settings.Default.LogFilePath + "\\TUI_DebugLog", sStatus);
            }
#else
            return;
#endif
        }

        #region Header Comments
        //*********************************************************************************************
        // btnConfigInfo_Click
        //
        // PURPOSE:   Handles the Config Info button.
        //            Displays the configuration for this application. 
        // 
        // USES:      
        // 
        //*********************************************************************************************
        #endregion
        private void btnConfigInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show(" RemotingTxPort: " + Properties.Settings.Default.RemotingTxPort.ToString()
                            + "\n RemotingTxService: " + Properties.Settings.Default.RemotingTxService
                            + "\n\n RemotingRxURL: " + Properties.Settings.Default.RemotingRxURL.ToString()
                            + "\n RemotingRxOnlineTimeout: " + Properties.Settings.Default.RemotingRxOnlineTimeout.ToString()
                            + "\n RemotingRXReplyTimeout: " + Properties.Settings.Default.RemotingRXReplyTimeout.ToString()
                            + "\n\n LogFilePath: " + Properties.Settings.Default.LogFilePath
                            + "\n RecoveryFilePath: " + Properties.Settings.Default.RecoveryFilePath
                            + "\n\n TopicConnection: " + Properties.Settings.Default.TopicConnection
                            + "\n TopicName: " + Properties.Settings.Default.TopicName
                            + "\n SubscriptionName: " + Properties.Settings.Default.SubscriptionName
                            + "\n\n GetMessagePollTime: " + Properties.Settings.Default.GetMessagePollTime.ToString()
                            + "\n\n MTRestURI: " + Properties.Settings.Default.MTRestURI
                            + "\n\n MSGDeviceIDPrefix: " + Properties.Settings.Default.MSGDeviceIDPrefix
                            , "TankUtilityInterface " + Properties.Settings.Default.ApplicationType + " Configuration");
        }

        #region Header Comments
        //*********************************************************************************************
        // btnTest_Click
        //
        // PURPOSE:   Handles the Test button. Button only visable in Debug build.
        //            Runs test code. 
        // 
        // USES:      
        // 
        //*********************************************************************************************
        #endregion
        private void btnTest_Click(object sender, EventArgs e)
        {
            // Close Mobile Originate Client
            if (null != mobileOriginateSubClient)
            {
                if (!mobileOriginateSubClient.IsClosed)
                {
                    mobileOriginateSubClient.Close();
                    return;
                }
            }         
        }

        #region Header Comments
        //*********************************************************************************************
        // FrmTankUtilityInterface_FormClosing
        //
        // PURPOSE:   Handles the form closing event.
        //            Unchecks "Enable Login" to cause a logout to be sent.
        //            If interface is not logged out then display message and cancel close.
        // 
        // USES:      
        // 
        //*********************************************************************************************
        #endregion
        private void FrmTankUtilityInterface_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If it's the user closing (not windows closing or task manager closing it)
            //  and if currently Subscription is open, then close it, cancel app closing and tell users to try again
            if (System.Windows.Forms.CloseReason.UserClosing == e.CloseReason)
            {
                if (null != mobileOriginateSubClient)
                {
                    if (!mobileOriginateSubClient.IsClosed)
                    {
                        mobileOriginateSubClient.Close();
                        e.Cancel = true;
                        MessageBox.Show("Please try again after subscription close is complete!", "TankUtilityInterface");
                        return;
                    }
                }
            }
            // Add line to log file that application is closing
            Log.ErrorToFile(Properties.Settings.Default.LogFilePath, "TankUtilityInterface.exe: Application Stopped, Due to: " + e.CloseReason.ToString());
        }
       
        #region Header Comments
        //*********************************************************************************************
        // bkgWorkerRx_DoWork
        //
        // PURPOSE: The bkgWorkerRx_DoWork logs in to NPhaseOne and polls for MO messages from NPhaseOne.
        // 
        // USES:      qReceive:
        //            eventNewRxWork:
        //            bEnableLogin:
        //            sLoginSessionToken:
        // 
        //*********************************************************************************************
        #endregion
        private void bkgWorkerRx_DoWork(object sender, DoWorkEventArgs e)
        {
            int iGetMessagePollTime = Convert.ToInt32(Properties.Settings.Default.GetMessagePollTime);
            mobileOriginateSubClient = SubscriptionClient.CreateFromConnectionString(Properties.Settings.Default.TopicConnection, Properties.Settings.Default.TopicName, Properties.Settings.Default.SubscriptionName);

            // Check for Rx Channel work to do forever
            for (; ; )
            {
                try
                {
                    // Wait for 5 seconds to retry creating subscription
                    eventNewRxWork.WaitOne(5000, false);
                    eventNewRxWork.Set();

                    // Initialize Azure Topic Subscription
                    OnMessageOptions options = new OnMessageOptions();
                    options.AutoComplete = true;
                    options.AutoRenewTimeout = TimeSpan.FromMinutes(1);                    
#if DEBUG
                    // For debugging only one call at a time
                    options.MaxConcurrentCalls = 1;
#else
                    options.MaxConcurrentCalls = 5;
#endif
                    // Init the connection to the Service Bus Topic
                   if (mobileOriginateSubClient.IsClosed)
                        mobileOriginateSubClient = SubscriptionClient.CreateFromConnectionString(Properties.Settings.Default.TopicConnection, Properties.Settings.Default.TopicName, Properties.Settings.Default.SubscriptionName);
                    // Create On Message Handler
                    mobileOriginateSubClient.OnMessage(message =>
                    {                      
                        string sTankDeviceIdPrefix = Properties.Settings.Default.MSGDeviceIDPrefix;                     
                        var sData = new StringBuilder();                                                                      
                        string messageJson = string.Empty;
                        CPMessage cpMsg = null;
                        Payload tuMessage = null;
                       
                        // If Client is closed then return
                        if (mobileOriginateSubClient.IsClosed)
                        {
                            AddlistViewStatusItem("OnMessage, SubscriptionClient Closed!", Color.Red);
                            return;
                        }                                             
                        try
                        {
                            Stream stream = message.GetBody<Stream>();
                            StreamReader reader = new StreamReader(stream);
                            messageJson = reader.ReadToEnd();
                            AddDebugLogItem(messageJson);
                            tuMessage = JsonConvert.DeserializeObject<Payload>(messageJson);
                            if (tuMessage == null)
                            {
                                AddlistViewStatusItem("OnMessage, DeserializeObject Error! Null object returned", Color.Red);
                                return;
                            }  
                            cpMsg = ProcessTUMessage(tuMessage);
                        }
                        catch (Exception ex1)
                        {
                            AddlistViewStatusItem("OnMessage, DeserializeObject Exception!: " + ex1.Message, Color.Red);
                            return;
                        }

                        // If UTCCreatedTimestamp is null then ignore message
                        if (null == cpMsg.dtUTCTimeStamp)
                        {
                            AddlistViewStatusItem("OnMessage, Null UTCCreatedTimestamp, Device: " + tuMessage.TankId, Color.Red);
                            return;
                        }
                        // Queue message and set new work event
                        qReceive.Enqueue(cpMsg);
                        eventNewRxWork.Set();
                        
                        // Display message
                        AddListViewMsgItem("Rx", tuMessage.ReceivedOn, tuMessage.TankId, messageJson, Color.Black);
                    }, options);
                    
                    // Check for SMS messages
                    try
                    {
                        CPMessage cpMsg;
                        for (; ; )
                        {
                            // Wait for notice of work to do or polling timeout occured
                            eventNewRxWork.WaitOne(iGetMessagePollTime, false);

                            // While this threads queue has items, process them
                            while (qReceive.Count > 0)
                            {
                                try
                                {
                                    cpMsg = (CPMessage)qReceive.Dequeue();
                                }
                                catch
                                {
                                    // If queue is empty then continue
                                    continue;
                                }

                                DeliverMessage(cpMsg);
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        AddlistViewStatusItem("bkgWorkerRx_DoWork, GetSmsMessages Exception!: " + ex1.Message, Color.Red);
                    }
                }
                catch (Exception ex1)
                {
                    AddlistViewStatusItem("bkgWorkerRx_DoWork, Unhandled Exception!: " + ex1.Message, Color.Red);
                } 
            }
        }
             
        private CPMessage ProcessTUMessage(Payload tuMessage)
        {
            // If not the "Tank" prefix in DeviceID then ignore message
            string sTankID = tuMessage.TankId;
            CommPkt.CPMessage cpMsg = new CPMessage();
            StringBuilder sData = new StringBuilder();
            sData.Append("[");

            foreach (string tankdatavalue in tuMessage.Data.Keys)
            {                
                if (tankdatavalue.ToLower().Contains("percent"))
                {
                    sData.Append("PC,");
                    sData.Append(tuMessage.Data[tankdatavalue]);
                }
                /*if (tankdatavalue.ToLower().Contains("temperature"))
                {
                    sData.Append(",TP,");
                    sData.Append(tuMessage.Data[tankdatavalue]);
                }*/
                if (tankdatavalue.ToLower().Contains("time"))
                {
                    cpMsg.dtUTCTimeStamp = Convert.ToDateTime(tuMessage.Data[tankdatavalue]);
                }
                /*
                if (tankdatavalue.ToLower().Contains("gross volume"))
                {
                    sData.Append(",GV,");
                    sData.Append(tuMessage.Data[tankdatavalue]);
                }
                if (tankdatavalue.ToLower().Contains("gross level"))
                {
                    sData.Append(",GL,");
                    sData.Append(tuMessage.Data[tankdatavalue]);
                }
               
                if (tankdatavalue.ToLower().Contains("net volume"))
                {
                    sData.Append(" NV,");
                    sData.Append(tuMessage.Data[tankdatavalue]);
                }
                if (tankdatavalue.ToLower().Contains("aux"))
                {
                    sData.Append(",AL,");
                    sData.Append(tuMessage.Data[tankdatavalue]);                               
                }
                if (tankdatavalue.ToLower().Contains("alarm"))
                {
                    sData.Append(",AB,");
                    sData.Append(tuMessage.Data[tankdatavalue]);                              
                } */
            }
           
            // Create message object for queueing          
            cpMsg.iSourceType = (int)CommPkt.Constants.SourceType.TUI;
            cpMsg.iDirectionType = (int)CommPkt.Constants.DirectionType.Rx;
            cpMsg.sSourceNumber = sTankID;
            cpMsg.iTranType = (int)CommPkt.Constants.TransType.Data;
            cpMsg.iSequence = 0;           
            cpMsg.iDataLen = sData.Length;
            cpMsg.iErrorCode = 0;
            cpMsg.iNetworkErrCode = 0;
            cpMsg.sData = sData.ToString();
            cpMsg.iSMPPChannelID = 0;
            return cpMsg;
        }

        private void bkgWorkerTx_DoWork(object sender, DoWorkEventArgs e)
        {
            CPMessage cpMsg;
            string sMTRestURI = Properties.Settings.Default.MTRestURI;

            // Check for Tx Channel work to do forever
            for (; ; )
            {
                try
                {
                    // Wait for notice of work to do or 5 second timeout occured
                    eventNewTxWork.WaitOne(5000, false);

                    // While this threads queue has items, process them
                    while (qTransmit.Count > 0)
                    {
                        try
                        {
                            cpMsg = (CPMessage)qTransmit.Dequeue();
                        }
                        catch
                        {
                            // If queue is empty then continue
                            continue;
                        }

                        // Else send message to Azure
                        try
                        {
                            string sFullURI = sMTRestURI + cpMsg.sSourceNumber;
                            string sBodyDecimal = "";
                            int iStart = 1;      // Ignore ">" character
                            byte bData;
                            try
                            {
                                for (int i = 0; i < (cpMsg.sData.Length / 2); i++)
                                {
                                    bData = byte.Parse(cpMsg.sData.Substring(iStart, 2), System.Globalization.NumberStyles.HexNumber);
                                    sBodyDecimal += bData.ToString() + ",";
                                    iStart += 2;
                                }
                                // Remove last comma
                                sBodyDecimal = sBodyDecimal.TrimEnd(',');
                            }
                            // Continue and ignore message if any parsing exceptions
                            catch (Exception ex1)
                            {
                                AddlistViewStatusItem("bkgWorkerTx_DoWork, Parse Exception!: " + ex1.Message, Color.Red);
                                continue;
                            }
                            // Turn off Expect:100-Continue header so it sends body with first request
                            ServicePointManager.Expect100Continue = false;
                            // Send message to "Azure" as REST POST
                            string sResult = null;
                            try
                            {
                                using (WebClient wcclient = new WebClient())
                                {
                                    wcclient.Headers[HttpRequestHeader.ContentType] = "text/plain";
                                    sResult = wcclient.UploadString(sFullURI, sBodyDecimal);
                                    // If any results other then "true" display it in an error
                                    if (sResult != "true")
                                    {
                                        AddlistViewStatusItem("bkgWorkerTx_DoWork, UploadString non-true Post Results: " + sResult, Color.Red);
                                    }
                                }
                                AddListViewMsgItem("Tx", DateTime.UtcNow, cpMsg.sSourceNumber, cpMsg.sData, Color.Black);
                            }
                            // Display any UploadString exceptions
                            catch (Exception ex1)
                            {
                                if (null != sResult)
                                {
                                    AddlistViewStatusItem("bkgWorkerTx_DoWork, UploadString Exception!: " + ex1.Message + " Post Results:" + sResult, Color.Red);
                                }
                                else
                                {
                                    AddlistViewStatusItem("bkgWorkerTx_DoWork, WebClient Exception!: " + ex1.Message, Color.Red);
                                }
                                AddListViewMsgItem("Tx Failed!", DateTime.UtcNow, cpMsg.sSourceNumber, cpMsg.sData, Color.Red);
                            }
                        }
                        catch (Exception ex2)
                        {
                            AddlistViewStatusItem("bkgWorkerTx_DoWork, Post Message Exception!: " + ex2.Message, Color.Red);
                        }
                    }
                }
                catch (Exception ex3)
                {
                    AddlistViewStatusItem("bkgWorkerTx_DoWork, Unhandled Exception!: " + ex3.Message, Color.Red);
                }
            }
        }

    }
}
