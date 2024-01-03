using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class ForecastPeriodCompetitorValueMetada
    {
        public int ID { get; set; }
        public Nullable<int> ForecastPeriodID { get; set; }
        [DisplayName("ผลดำเนินงานของคู่เทียบ/คู่แข่ง")]
        public string Detail { get; set; }
    }
    [MetadataType(typeof(ForecastPeriodCompetitorValueMetada))]
    public partial class ForecastPeriodCompetitorValue
    {
        public bool IsDeleteCompetitorValue {  get; set; }
    }
}