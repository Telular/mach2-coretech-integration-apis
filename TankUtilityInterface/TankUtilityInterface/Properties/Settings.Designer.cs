﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TankUtilityInterface.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Development")]
        public string ApplicationType {
            get {
                return ((string)(this["ApplicationType"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\Telchifil001\\Share\\DevLogFiles\\CommPktMgrSP")]
        public string LogFilePath {
            get {
                return ((string)(this["LogFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\Telchifil001\\Share\\DevLogFiles\\CommPktMgrSP")]
        public string RecoveryFilePath {
            get {
                return ((string)(this["RecoveryFilePath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("tcp://VTLAppsD1:2058/CPInterface")]
        public string RemotingRxURL {
            get {
                return ((string)(this["RemotingRxURL"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50980")]
        public string RemotingTxPort {
            get {
                return ((string)(this["RemotingTxPort"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("AzureInterface")]
        public string RemotingTxService {
            get {
                return ((string)(this["RemotingTxService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public string RemotingRxOnlineTimeout {
            get {
                return ((string)(this["RemotingRxOnlineTimeout"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public string RemotingRXReplyTimeout {
            get {
                return ((string)(this["RemotingRXReplyTimeout"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10000")]
        public string GetMessagePollTime {
            get {
                return ((string)(this["GetMessagePollTime"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://mach2-coretech-mtwebapi-development.azurewebsites.net/api/passthrough/New/" +
            "urn:TankLTE:esn:")]
        public string MTRestURI {
            get {
                return ((string)(this["MTRestURI"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("urn:TankLTE:esn:")]
        public string MSGDeviceIDPrefix {
            get {
                return ((string)(this["MSGDeviceIDPrefix"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("TANK-UTILITY-READINGS")]
        public string SubscriptionName {
            get {
                return ((string)(this["SubscriptionName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Endpoint=sb://mach2-coretech-servicebus-development.servicebus.windows.net/;Share" +
            "dAccessKeyName=RootSendListenOnlyAccessKey;SharedAccessKey=YA4CVZJXDDcwI6YaHQ8/O" +
            "OXLzP4IMwg0itsalB/tSiQ=;EntityPath=mo-devicemessages-tankutlity-topic")]
        public string TopicConnection {
            get {
                return ((string)(this["TopicConnection"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("mo-devicemessages-tankutlity-topic")]
        public string TopicName {
            get {
                return ((string)(this["TopicName"]));
            }
        }
    }
}