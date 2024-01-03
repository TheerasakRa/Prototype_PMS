using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Prototype_PMS.Models
{
    public class ForecastPerodResultRemarkMetadata
    {
        public int ID { get; set; }
        public Nullable<int> ForecastPeriodID { get; set; }
        public string HelpImproveIndicator { get; set; }
        public string ProblemAndCorrection { get; set; }
        public string ImprotantFactorsAndEvents { get; set; }
        [DisplayName("5.ในกรณีที่มีการเปลี่ยนแปลงเครื่องมือระหว่างปีโปรดระบุเหตุผล/สาเหตุในการเปลี่ยนแปลงด้วย เช่น พบว่าค่าคาดการณ์มีความคลาดเคลื่อนไม่เเม่นยำ เป็นต้น")]
        public string ReasonForToolChange { get; set; }
        [DisplayName("ผลการวิเคราะห์-ประเมิน-จัดการความเสี่ยง ณ งวด")]
        public Nullable<bool> IsAnalysisResults { get; set; }
        [DisplayName("ผลการวิเคราะห์-ประเมิน-จัดการความเสี่ยง ณ งวด")]
        public string AnalysisResults { get; set; }
        [DisplayName("ผลการวิเคราะห์-ประเมิน-จัดการความเสี่ยง ณ งวด")]
        public Nullable<bool> IsChangeActionPlan { get; set; }
        [DisplayName("ทบทวน/หรือปรับปรุงและหรือแผนปฏิบัติการ")]
        public string ChangeActionPlan { get; set; }
        [DisplayName("ทบทวน/หรือปรับปรุงและหรือแผนปฏิบัติการ")]
        public Nullable<bool> IsChangeOperation { get; set; }
        [DisplayName("เร่งรัด/ปรับปรุงการดำเนินงาน")]
        public string ChangeOperation { get; set; }
        [DisplayName("อื่นๆ โปรดระบุ")]
        public Nullable<bool> IsOther { get; set; }
        [DisplayName("อื่นๆ โปรดระบุ")]
        public string Other { get; set; }
    }
    [MetadataType(typeof(ForecastPerodResultRemarkMetadata))]
    public partial class ForecastPeriodResultRemark
    {
        public List<HttpPostedFileBase> ListFilePeriodDocs { get; set; }
        public List<HttpPostedFileBase> ListFileAnalysis { get; set; }
        public List<HttpPostedFileBase> ListFileActionPlan { get; set; }
    }
}
