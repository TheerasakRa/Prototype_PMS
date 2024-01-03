using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class ImportantIndicatorResultMeasurementMetadata
    {
        public int ID { get; set; }
        public Nullable<int> IndicatorID { get; set; }
        [DisplayName("ปีงบประมาณ")]
        public string Year { get; set; }
        [DisplayName("ความถึ่ในการคาดการณ์")]
        public Nullable<int> PeriodMountOrQauterOrYearID { get; set; }
        [DisplayName("งวดการคาดการณ์")]
        public virtual ICollection<ForecastPeriod> ForecastPeriods { get; set; }
    }
    [MetadataType(typeof(ImportantIndicatorResultMeasurementMetadata))]
    public partial class ImportantIndicatorResultMeasurement
    {
        public string PMQY { get; set; }
    }
}