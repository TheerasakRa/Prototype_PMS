using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class ForecastPeriodMetadata
    {
        public int ID { get; set; }
        public Nullable<int> ImportantIndicatorResultMeasurementID { get; set; }
        [DisplayName("ผลดำเนินงานของคู่เทียบ/คู่แข่ง (หน่วยงาน/องค์กร/บริษัท)")]
        public virtual ICollection<ForecastPeriodCompetitorValue> ForecastPeriodCompetitorValues { get; set; }
    }
    [MetadataType(typeof(ForecastPeriodMetadata))]
    public partial class ForecastPeriod
    {
        public bool IsSelect {  get; set; }
        public bool IsAddCompetitor { get; set; }   
    }
}