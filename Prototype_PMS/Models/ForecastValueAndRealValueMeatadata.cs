using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class ForecastValueAndRealValueMeatadata
    {
        public int ID { get; set; }
        [DisplayName("หน่วยวัด")]
        public Nullable<int> ForecastPeriodID { get; set; }
        [DisplayName("ผลดำเนินงาน ณ งวดที่ทำการคาดการณ์")]
        public Nullable<double> ForecastValue { get; set; }
        [DisplayName("ค่าคาดารณ์ผลดำเนินงานจริง ณ 30 ก.ย.")]
        public Nullable<double> RealValue { get; set; }
    }
    [MetadataType(typeof(ForecastValueAndRealValueMeatadata))]
    public partial class ForecastValueAndRealValue
    {
        public int unitIndex { get; set; }
    }
}