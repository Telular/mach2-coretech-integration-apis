using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Configuration;
using CommPkt;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Net.Http;
//using System.Net.Http.Headers;
using System.Net;
//using Newtonsoft.Json;
using Microsoft.ServiceBus.Messaging;
//using System.IO;

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

        public TankUtilityTopicSubReader tankReadingsReciever;

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
                this.tankReadingsReciever.AddlistViewStatusItem("DeliverMessage Error! " + ex.Message + ", Remoting Failed", Color.Red);
            }
            return;
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
        private async void  bkgWorkerRx_DoWork(object sender, DoWorkEventArgs e)
        {
            int iGetMessagePollTime = Convert.ToInt32(Properties.Settings.Default.GetMessagePollTime);

            // Check for Rx Channel work to do forever
            for (; ; )
            {
                try
                {
                    // Wait for 5 seconds to retry creating subscription
                    eventNewRxWork.WaitOne(5000, false);
                    eventNewRxWork.Set();

                    var readingSubscriptionName = Properties.Settings.Default.SubscriptionName;

                    tankReadingsReciever = new TankUtilityTopicSubReader(readingSubscriptionName, this.listViewMsg, this.listViewStatus);

                    await tankReadingsReciever.ReceiveMessagesLoopAsync();

                    await tankReadingsReciever.CloseMessagePumpAsync();

                }
                catch (Exception ex1)
                {
                    this.tankReadingsReciever.AddlistViewStatusItem("bkgWorkerRx_DoWork, Unhandled Exception!: " + ex1.Message, Color.Red);
                }
            }
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
                                this.tankReadingsReciever.AddlistViewStatusItem("bkgWorkerTx_DoWork, Parse Exception!: " + ex1.Message, Color.Red);
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
                                        this.tankReadingsReciever.AddlistViewStatusItem("bkgWorkerTx_DoWork, UploadString non-true Post Results: " + sResult, Color.Red);
                                    }
                                }
                                this.tankReadingsReciever.AddListViewMsgItem("Tx", DateTime.UtcNow, cpMsg.sSourceNumber, cpMsg.sData, Color.Black);
                            }
                            // Display any UploadString exceptions
                            catch (Exception ex1)
                            {
                                if (null != sResult)
                                {
                                    this.tankReadingsReciever.AddlistViewStatusItem("bkgWorkerTx_DoWork, UploadString Exception!: " + ex1.Message + " Post Results:" + sResult, Color.Red);
                                }
                                else
                                {
                                    this.tankReadingsReciever.AddlistViewStatusItem("bkgWorkerTx_DoWork, WebClient Exception!: " + ex1.Message, Color.Red);
                                }
                                this.tankReadingsReciever.AddListViewMsgItem("Tx Failed!", DateTime.UtcNow, cpMsg.sSourceNumber, cpMsg.sData, Color.Red);
                            }
                        }
                        catch (Exception ex2)
                        {
                            this.tankReadingsReciever.AddlistViewStatusItem("bkgWorkerTx_DoWork, Post Message Exception!: " + ex2.Message, Color.Red);
                        }
                    }
                }
                catch (Exception ex3)
                {
                    this.tankReadingsReciever.AddlistViewStatusItem("bkgWorkerTx_DoWork, Unhandled Exception!: " + ex3.Message, Color.Red);
                }
            }
        }

    }
}
