using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class IndicatorUnitMetadata
    {
        public int ID { get; set; }
        public int IndicatorID { get; set; }
        [DisplayName("หน่วยวัด")]
        public string Unit { get; set; }
    }
    [MetadataType(typeof(IndicatorUnitMetadata))]
    public partial class IndicatorUnit
    {
        public bool isUnitDelete { get; set; }

    }
}