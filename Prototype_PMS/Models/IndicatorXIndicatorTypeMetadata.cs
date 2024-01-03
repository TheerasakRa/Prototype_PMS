using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class IndicatorXIndicatorTypeMetadata
    {
        public int ID { get; set; }
        public Nullable<int> IndicatorID { get; set; }
        public Nullable<int> IndicatorTypeID { get; set; }
        public Nullable<bool> isCheck { get; set; }
        [DisplayName("คำจำกัดความตัวชี้วัด")]
        public string Definition { get; set; }
    }
    [MetadataType(typeof(IndicatorXIndicatorTypeMetadata))]
    public partial class IndicatorXIndicatorType
    {
        public bool isDeleteFormOwner { get; set; }

    }
}