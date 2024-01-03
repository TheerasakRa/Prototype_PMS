using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Prototype_PMS.Models
{
    public class IndicatorMetadata
    {
        public int ID { get; set; }
        [DisplayName("ตัวชี้วัด/เกณฑ์วัดการดำเนินงาน")]
        public string Indicator1 { get; set; }
        [DisplayName("กำหนดสูตรการคำนวน")]
        public string Formula { get; set; }
        [DisplayName("รายละเอียดตัวชี้วัด")]
        public Nullable<int> IndicatorDetailStatusID { get; set; }
        [DisplayName("สถานะของตัวชี้วัด")]
        public Nullable<bool> isActive { get; set; }


        [Display(Name = "วันที่ปรับปรุง")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [DisplayName("หน่วยงานผู้รับผิดชอบตัวชีว้ัด")]
        public virtual ICollection<IndicatorOwner> IndicatorOwners { get; set; }
        [DisplayName("ประเภทตัวชี้วัด")]
        public virtual ICollection<IndicatorUnit> IndicatorUnits { get; set; }
        [DisplayName("หน่วยวัด")]
        public virtual ICollection<IndicatorXIndicatorType> IndicatorXIndicatorTypes { get; set; }
    }
    [MetadataType(typeof(IndicatorMetadata))]
    public partial class Indicator
    {
        public string IsActiveText
        {
            get { return isActive.GetValueOrDefault() ? "ใช้งาน" : "ไม่ใช้งาน"; }
        }
        //public string IndicatorOwnersText
        //{
        //    get
        //    {
        //        var text = "";
        //        foreach (var item in IndicatorOwners)
        //        {
        //            text = text + " " + item.Division;
        //        }
        //        return text;
        //    }
        //}
        public int? indicatorYear { get; set; }
        public int PeriodSelected_Index { get; set; }
        public int ForeCastPeriodSelected_Index { get; set; }
    }
}