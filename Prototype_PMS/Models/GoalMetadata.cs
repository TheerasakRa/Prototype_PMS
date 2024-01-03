using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class GoalMetadata
    {
        public int ID { get; set; }
        [DisplayName("รหัส")]
        public Nullable<int> No { get; set; }
        [DisplayName("เป้าหมายและวัตถุประสงค์ (Goals)")]
        public string Goal1 { get; set; }
        public Nullable<int> StrategicObjectiveID { get; set; }


    }
    [MetadataType(typeof(GoalMetadata))]
    public partial class Goal
    {
        public bool isDeleteForm { get; set; }
        public bool IsAddIndicator { get; set; }
        public bool IsDeleteIndicator { get; set; }
    }
}