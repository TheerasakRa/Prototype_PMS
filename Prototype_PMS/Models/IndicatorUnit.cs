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
    
    public partial class IndicatorUnit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IndicatorUnit()
        {
            this.ImportantIndicatorTargetMeasuerments = new HashSet<ImportantIndicatorTargetMeasuerment>();
            this.SOEPlanIndicators = new HashSet<SOEPlanIndicator>();
        }
    
        public int ID { get; set; }
        public int IndicatorID { get; set; }
        public string Unit { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> isDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImportantIndicatorTargetMeasuerment> ImportantIndicatorTargetMeasuerments { get; set; }
        public virtual Indicator Indicator { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOEPlanIndicator> SOEPlanIndicators { get; set; }
    }
}
