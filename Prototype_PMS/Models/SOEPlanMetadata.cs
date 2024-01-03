using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class SOEPlanMetadata
    {
        
        public int ID { get; set; }
        [DisplayName("ระยะเวลาแผน")]
        public int StartYear { get; set; }
        [DisplayName("ระยะเวลาแผน")]
        public int EndYear { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public int CreateDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public int UpdateDate { get; set; }
    }
    [MetadataType(typeof(SOEPlanMetadata))]
    public partial class SOEPlan
    {
        public string StartEndYear
        {
            get
            {
                string x = StartYear + " - " + EndYear;
                return x;
            }
        }
    }
}