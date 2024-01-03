using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class ForecastPeriodToolAndMethodMetadata
    {
        [DisplayName("เครื่องมือ")]
        public Nullable<int> ForecastToolID { get; set; }

        [DisplayName("แบบจำลอง")]
        public string Method { get; set; }
    }
    [MetadataType(typeof(ForecastPeriodToolAndMethodMetadata))]
    public partial class ForecastPeriodToolAndMethod
    {
        public string ToolName { get; set; }
        public bool IsOtherTool { get; set; }

    }
}