using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class StrategicObjectiveMetadata
    {
        public int ID { get; set; }
        public Nullable<int> No { get; set; }

        [DisplayName("วัตถุประสงค์เชิงยุทธศาสตร์ (Strategic Objective)")]
        public string StrategicObjective1 { get; set; }
        public Nullable<int> SOEPlanID { get; set; }

        [DisplayName("เป้าหมาย (Goals)")]
        public virtual ICollection<Goal> Goals { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public int UpdateDate { get; set; }
    }
    [MetadataType(typeof(StrategicObjectiveMetadata))]
    public partial class StrategicObjective
    {

    }
}