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
    
    public partial class ForecastPeriodResultRemark
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ForecastPeriodResultRemark()
        {
            this.ForecastAnalysisResultFiles = new HashSet<ForecastAnalysisResultFile>();
            this.ForecastChangeActionPlanFiles = new HashSet<ForecastChangeActionPlanFile>();
            this.ForecastPeriodDocFiles = new HashSet<ForecastPeriodDocFile>();
        }
    
        public int ID { get; set; }
        public Nullable<int> ForecastPeriodID { get; set; }
        public string HelpImproveIndicator { get; set; }
        public string ProblemAndCorrection { get; set; }
        public string ImprotantFactorsAndEvents { get; set; }
        public string ReasonForToolChange { get; set; }
        public bool IsAnalysisResults { get; set; }
        public string AnalysisResults { get; set; }
        public bool IsChangeActionPlan { get; set; }
        public string ChangeActionPlan { get; set; }
        public bool IsChangeOperation { get; set; }
        public string ChangeOperation { get; set; }
        public bool IsOther { get; set; }
        public string Other { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<bool> IsLastDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastAnalysisResultFile> ForecastAnalysisResultFiles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastChangeActionPlanFile> ForecastChangeActionPlanFiles { get; set; }
        public virtual ForecastPeriod ForecastPeriod { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastPeriodDocFile> ForecastPeriodDocFiles { get; set; }
    }
}
