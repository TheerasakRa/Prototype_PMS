//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Prototype_PMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SOEPlan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SOEPlan()
        {
            this.StrategicObjectives = new HashSet<StrategicObjective>();
        }
    
        public int ID { get; set; }
        public Nullable<int> StartYear { get; set; }
        public Nullable<int> EndYear { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> isDelete { get; set; }
        public Nullable<bool> isLastDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StrategicObjective> StrategicObjectives { get; set; }
    }
}