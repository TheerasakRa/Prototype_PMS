using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class ImportantIndicatorTargetMeasuermentMetadata
    {
    }
    [MetadataType(typeof(ImportantIndicatorTargetMeasuermentMetadata))]
    public partial class ImportantIndicatorTargetMeasuerment
    {
        public int IndicatorXIndicatorTypeID { get; set; }
        public List<ImportantIndicatorTargetMeasuerment> SubTarget { get; set; }
        public bool isDeleteImportant { get; set; }
        public bool IsDisplay { get; set; }
        public bool IsUnCheck { get; set; }
    }
}