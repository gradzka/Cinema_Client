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
    
    public partial class PROGRAM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PROGRAM()
        {
            this.RESERVATIONS = new HashSet<RESERVATIONS>();
        }
    
        public short ID_PROGRAM { get; set; }
        public System.DateTime DATE { get; set; }
        public System.TimeSpan TIME { get; set; }
        public string ID_HALL { get; set; }
        public string ID_MOVIE { get; set; }
        public string C2D_3D { get; set; }
        public string VERSION { get; set; }
    
        public virtual HALLS HALLS { get; set; }
        public virtual MOVIES MOVIES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESERVATIONS> RESERVATIONS { get; set; }
    }
}