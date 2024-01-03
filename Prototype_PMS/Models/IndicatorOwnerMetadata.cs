using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class IndicatorOwnerMetadata
    {
        public int ID { get; set; }
        public int IndicatorID { get; set; }
        [DisplayName("หน่วยงาน")]
        public string Division { get; set; }
    }
    [MetadataType(typeof(IndicatorOwnerMetadata))]
    public partial class IndicatorOwner
    {
        public bool isOwnerDelete{ get; set; }
    }
}