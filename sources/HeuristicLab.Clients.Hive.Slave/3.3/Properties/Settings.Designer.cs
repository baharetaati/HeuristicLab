﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.530
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HeuristicLab.Clients.Hive.SlaveCore.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:40")]
        public global::System.TimeSpan HeartbeatInterval {
            get {
                return ((global::System.TimeSpan)(this["HeartbeatInterval"]));
            }
            set {
                this["HeartbeatInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("lastUsed.dat")]
        public string LastUsedFileName {
            get {
                return ((string)(this["LastUsedFileName"]));
            }
            set {
                this["LastUsedFileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PluginCache")]
        public string PluginCacheDir {
            get {
                return ((string)(this["PluginCacheDir"]));
            }
            set {
                this["PluginCacheDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PluginTemp")]
        public string PluginTempBaseDir {
            get {
                return ((string)(this["PluginTempBaseDir"]));
            }
            set {
                this["PluginTempBaseDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Configuration")]
        public string ConfigurationName {
            get {
                return ((string)(this["ConfigurationName"]));
            }
            set {
                this["ConfigurationName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HeuristicLab.PluginInfrastructure-3.3.dll")]
        public string PluginInfrastructureDll {
            get {
                return ((string)(this["PluginInfrastructureDll"]));
            }
            set {
                this["PluginInfrastructureDll"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ICSharpCode.SharpZipLib.dll")]
        public string SharpZipLibDll {
            get {
                return ((string)(this["SharpZipLibDll"]));
            }
            set {
                this["SharpZipLibDll"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ICSharpCode.SharpZipLib License.txt")]
        public string SharpZipLibLicense {
            get {
                return ((string)(this["SharpZipLibLicense"]));
            }
            set {
                this["SharpZipLibLicense"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HeuristicLab.Clients.Hive.SlaveCore-3.3.dll")]
        public string ClientsHiveSlaveCoreDll {
            get {
                return ((string)(this["ClientsHiveSlaveCoreDll"]));
            }
            set {
                this["ClientsHiveSlaveCoreDll"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HeuristicLab.Clients.Hive-3.3.dll")]
        public string ClientsHiveDll {
            get {
                return ((string)(this["ClientsHiveDll"]));
            }
            set {
                this["ClientsHiveDll"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HeuristicLab.Hive-3.3.dll")]
        public string HiveDll {
            get {
                return ((string)(this["HiveDll"]));
            }
            set {
                this["HiveDll"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HeuristicLab.Clients.Common-3.3.dll")]
        public string ClientsCommonDll {
            get {
                return ((string)(this["ClientsCommonDll"]));
            }
            set {
                this["ClientsCommonDll"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int PluginLifetime {
            get {
                return ((int)(this["PluginLifetime"]));
            }
            set {
                this["PluginLifetime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:01")]
        public global::System.TimeSpan DirOpSleepTime {
            get {
                return ((global::System.TimeSpan)(this["DirOpSleepTime"]));
            }
            set {
                this["DirOpSleepTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int PluginDeletionRetries {
            get {
                return ((int)(this["PluginDeletionRetries"]));
            }
            set {
                this["PluginDeletionRetries"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:01")]
        public global::System.TimeSpan PluginDeletionTimeout {
            get {
                return ((global::System.TimeSpan)(this["PluginDeletionTimeout"]));
            }
            set {
                this["PluginDeletionTimeout"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:46")]
        public global::System.TimeSpan ExecutorSemTimeouts {
            get {
                return ((global::System.TimeSpan)(this["ExecutorSemTimeouts"]));
            }
            set {
                this["ExecutorSemTimeouts"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5000")]
        public int QueuesMaxThreads {
            get {
                return ((int)(this["QueuesMaxThreads"]));
            }
            set {
                this["QueuesMaxThreads"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SlaveCommunicationServiceEndpoint")]
        public string SlaveCommunicationServiceEndpoint {
            get {
                return ((string)(this["SlaveCommunicationServiceEndpoint"]));
            }
            set {
                this["SlaveCommunicationServiceEndpoint"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("hiveslave")]
        public string SlaveUser {
            get {
                return ((string)(this["SlaveUser"]));
            }
            set {
                this["SlaveUser"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("hiveslave")]
        public string SlavePwd {
            get {
                return ((string)(this["SlavePwd"]));
            }
            set {
                this["SlavePwd"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int MaxLogCount {
            get {
                return ((int)(this["MaxLogCount"]));
            }
            set {
                this["MaxLogCount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("00:00:20")]
        public global::System.TimeSpan ExecutorQueueTimeout {
            get {
                return ((global::System.TimeSpan)(this["ExecutorQueueTimeout"]));
            }
            set {
                this["ExecutorQueueTimeout"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int NrOfCoresToScavenge {
            get {
                return ((int)(this["NrOfCoresToScavenge"]));
            }
            set {
                this["NrOfCoresToScavenge"] = value;
            }
        }
    }
}
