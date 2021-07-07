//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MEMT_KeepAlive.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class KeepAlive
    {
        public string ServerIP { get; set; }
        public Nullable<int> ServerPort { get; set; }
        public string ServerName { get; set; }
        public Nullable<bool> ServerBlock { get; set; }
        public Nullable<bool> ServiceAlive { get; set; }
        public Nullable<System.DateTime> ServiceAliveTime { get; set; }
        public bool PrimaryServer { get; set; }
        public Nullable<int> Action { get; set; }
        public Nullable<int> ActionStatus { get; set; }
    
        public virtual KeepAction KeepAction { get; set; }
        public virtual KeepActionStatus KeepActionStatus { get; set; }
    }
}
