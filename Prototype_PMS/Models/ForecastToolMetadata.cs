using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class ForecastToolMetadata
    {
        public int ID { get; set; }
        public string ForecastTool1 { get; set; }
    }
    [MetadataType(typeof(ForecastToolMetadata))]
    public partial class ForecastTool
    {
        public string ToolName { get; set; }

    }
}