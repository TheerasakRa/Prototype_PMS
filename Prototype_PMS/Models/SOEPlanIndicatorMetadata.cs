using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Prototype_PMS.Models
{
    public class SOEPlanIndicatorMetadata
    {

    }
    [MetadataType(typeof(SOEPlanIndicatorMetadata))]
    public partial class SOEPlanIndicator
    {
        public IEnumerable<SelectListItem> IndicatorItem { get; set; }
        public IEnumerable<SelectListItem> IndicatorUnitItem { get; set; }
    }
}