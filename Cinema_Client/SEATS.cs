//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cinema_Client
{
    using System;
    using System.Collections.Generic;
    
    public partial class SEATS
    {
        public string ID_SEAT { get; set; }
        public string ID_HALL { get; set; }
        public bool VIP { get; set; }
    
        public virtual HALLS HALLS { get; set; }
    }
}