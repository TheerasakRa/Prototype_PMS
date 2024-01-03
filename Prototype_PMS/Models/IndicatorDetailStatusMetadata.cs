using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class IndicatorDetailStatusMetadata
    {
        public int ID { get; set; }
        public string Status { get; set; }
    }
    [MetadataType(typeof(IndicatorDetailStatusMetadata))]
    public partial class IndicatorDetailStatu
    {

    }
}