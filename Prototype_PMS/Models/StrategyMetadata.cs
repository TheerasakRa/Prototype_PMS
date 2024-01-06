using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class StrategyMetadata
    {
        public int ID { get; set; }
        [DisplayName("รหัส")]
        public Nullable<int> No { get; set; }
        [DisplayName("ยุทธ์ศาสตร์ (Stretegy)")]
        public string Strategy1 { get; set; }
        public Nullable<int> StrategicObjectiveID { get; set; }
        [DisplayName("กลยุทธ์ (Tactic)")]
        public virtual ICollection<Tactic> Tactics { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
    [MetadataType(typeof(StrategyMetadata))]
    public partial class Strategy
    {

    }
}