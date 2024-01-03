using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class TacticMetadata
    {
        public int ID { get; set; }
        [DisplayName("รหัส")]
        public Nullable<int> No { get; set; }
        public string Tactic1 { get; set; }
        [DisplayName("กลยุทธ์ (Tactic)")]
        public Nullable<int> StrategyID { get; set; }
    }
    [MetadataType(typeof(TacticMetadata))]
    public partial class Tactic
    {
        public bool isDeleteFormTactic { get; set; }
    }
}