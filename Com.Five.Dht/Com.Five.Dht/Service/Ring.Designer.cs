﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Com.Five.Dht.Service {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Ring : global::System.Configuration.ApplicationSettingsBase {
        
        private static Ring defaultInstance = ((Ring)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Ring())));
        
        public static Ring Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("64")]
        public byte MaxNoOfBits {
            get {
                return ((byte)(this["MaxNoOfBits"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int NoOfSuccessors {
            get {
                return ((int)(this["NoOfSuccessors"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Com.Five.Dht.ServiceImpl.FactoryImpl.NodeRingFactory, Com.Five.Dht")]
        public string RingFactory {
            get {
                return ((string)(this["RingFactory"]));
            }
        }
    }
}