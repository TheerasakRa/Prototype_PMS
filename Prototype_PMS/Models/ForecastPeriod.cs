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
    
    public partial class ForecastPeriod
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ForecastPeriod()
        {
            this.ForecastPeriodCompetitorValues = new HashSet<ForecastPeriodCompetitorValue>();
            this.ForecastValueAndRealValues = new HashSet<ForecastValueAndRealValue>();
            this.ForecastPeriodResultRemarks = new HashSet<ForecastPeriodResultRemark>();
            this.ForecastPeriodToolAndMethods = new HashSet<ForecastPeriodToolAndMethod>();
        }
    
        public int ID { get; set; }
        public Nullable<int> ImportantIndicatorResultMeasurementID { get; set; }
        public string MonthOrQuaterOrYear { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<bool> IsLastDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastPeriodCompetitorValue> ForecastPeriodCompetitorValues { get; set; }
        public virtual ImportantIndicatorResultMeasurement ImportantIndicatorResultMeasurement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastValueAndRealValue> ForecastValueAndRealValues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastPeriodResultRemark> ForecastPeriodResultRemarks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastPeriodToolAndMethod> ForecastPeriodToolAndMethods { get; set; }
    }
}
