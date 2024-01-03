using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class IndicatorTypeMetadata
    {
        public int ID { get; set; }
        public string IndicatorType1 { get; set; }
    }
    [MetadataType(typeof(IndicatorTypeMetadata))]
    public partial class IndicatorType
    {

    }
}