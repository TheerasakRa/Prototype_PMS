using Prototype_PMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Prototype_PMS.Controllers
{
    public class GoalsController : Controller
    {
        private PMSEntities1 db = new PMSEntities1();
        private static int? _StrategicObjectiveID;
        // GET: Goals
        public ActionResult Manage(int? StrategicObjectiveID)
        {
            var indicators = db.Indicators.Where(s => s.isDelete == false);

            StrategicObjective strategicObjective = db.StrategicObjectives.Find(StrategicObjectiveID);

            _StrategicObjectiveID = StrategicObjectiveID;
            if (strategicObjective.Goals.Count == 0)
            {
                Information(strategicObjective);
            }

            ViewBag.TitleStr = strategicObjective.StrategicObjective1;
            IndicatorUnitData(strategicObjective);
            IndicatorData(strategicObjective);
            return View("Manage", strategicObjective);
        }

        [HttpPost]
        public ActionResult AddGoal(StrategicObjective strategicObjective)
        {
            // เพิ่มข้อมูล Goal
            Information(strategicObjective);

            // อัพเดทข้อมูล IndicatorUnit และ Indicator
            IndicatorUnitData(strategicObjective);
            IndicatorData(strategicObjective);

            // ส่งข้อมูลที่ต้องการใน View
            ViewBag.TitleStr = strategicObjective.StrategicObjective1;

            ModelState.Clear();
            return View("Manage", strategicObjective);
        }
        [HttpPost]
        public ActionResult AddIndicator(StrategicObjective strategicObjective)
        {
            foreach (var goal in strategicObjective.Goals)
            {
                if (goal.IsAddIndicator)
                {
                    // สร้าง SOEPlanIndicator และกำหนดค่า
                    SOEPlanIndicator SOEPlanIndicator = new SOEPlanIndicator();
                    SOEPlanIndicator.CreateDate = DateTime.Now;
                    SOEPlanIndicator.UpdateDate = DateTime.Now;
                    SOEPlanIndicator.IsDelete = false;
                    SOEPlanIndicator.IsLastDelete = false;
                    SOEPlanIndicator.GoalID = goal.ID;

                    // ตรวจสอบว่ามี SOEPlanIndicators ใน Goal หรือไม่
                    if (goal.SOEPlanIndicators.Any())
                    {
                        // นับ No ต่อจาก No ของ SOEPlanIndicator ล่าสุด
                        var lastNo = goal.SOEPlanIndicators.Max(si => si.No);
                        SOEPlanIndicator.No = lastNo + 1;
                    }
                    else
                    {
                        // หา No ของ Goal ล่าสุด
                        var lastGoal = db.Goals.OrderByDescending(g => g.No).FirstOrDefault();
                        var lastGoalNo = lastGoal?.No ?? 0;

                        // นับ No ต่อจาก No ของ Goal ล่าสุด
                        SOEPlanIndicator.No = lastGoalNo + 1;
                    }

                    SOEPlanIndicator.GoalID = goal.ID;
                    goal.SOEPlanIndicators.Add(SOEPlanIndicator);
                    goal.IsAddIndicator = false;
                }
            }

            IndicatorUnitData(strategicObjective);
            IndicatorData(strategicObjective);
            ModelState.Clear();
            return View("Manage", strategicObjective);
        }


        [HttpPost]
        public ActionResult DeleteIndicator(StrategicObjective strategicObjective)
        {
            // ล้าง ModelState
            ModelState.Clear();

            // ลบ Indicator ที่ถูกเลือกในแต่ละ Goal
            foreach (var goal in strategicObjective.Goals.ToList())
            {
                if (goal.IsDeleteIndicator)
                {
                    if (goal.ID == 0)
                    {
                        strategicObjective.Goals.Remove(goal);
                    }
                    else
                    {
                        goal.IsDeleteIndicator = true;
                        goal.SOEPlanIndicators.Clear();
                    }
                }
            }

            // กรอง Goals ที่ถูกลบ
            strategicObjective.Goals = strategicObjective.Goals.Where(g => !g.IsDeleteIndicator || g.ID == 0).ToList();

            // อัพเดทข้อมูล Indicator และ IndicatorUnit
            IndicatorData(strategicObjective);
            IndicatorUnitData(strategicObjective);
            return View("Manage", strategicObjective);
        }

        [HttpPost]
        public ActionResult Manage(StrategicObjective strategicObjective)
        {
            SaveDataSOEGoal(strategicObjective.Goals);
            db.SaveChanges();
            IndicatorData(strategicObjective);
            IndicatorUnitData(strategicObjective);
            return RedirectToAction("Index", "StrategicObjectives", new { SOEPlanID = strategicObjective.SOEPlanID });
        }

        private void SaveDataSOEGoal(ICollection<Goal> goals)
        {
            foreach (var goal in goals)
            {
                if (goal.ID == 0)
                {
                    if (goal.isDelete != true)
                    {
                        goal.isDelete = false;
                        goal.isLastDelete = false;
                        goal.CreateDate = DateTime.Now;
                        goal.UpdateDate = DateTime.Now;
                        db.Goals.Add(goal);
                    }
                }
                else
                {
                    goal.UpdateDate = DateTime.Now;
                    db.Entry(goal).State = EntityState.Modified;
                }

                foreach (var indicator in goal.SOEPlanIndicators)
                {
                    if (indicator.ID == 0)
                    {
                        if (indicator.IsDelete != true)
                        {
                            indicator.IsDelete = false;
                            indicator.IsLastDelete = false;
                            indicator.CreateDate = DateTime.Now;
                            indicator.UpdateDate = DateTime.Now;
                            indicator.GoalID = goal.ID;
                            db.SOEPlanIndicators.Add(indicator);
                        }
                    }
                    else
                    {
                        db.Entry(indicator).State = EntityState.Modified;
                    }
                }
            }
        }


        [HttpPost, ActionName("DeleteForm")]
        public ActionResult DeleteForm(StrategicObjective strategicObjective)
        {
            // ค้นหาและลบ Goal ที่ถูกลบ
            FindFormDelete(strategicObjective.Goals);
            // กรอง Goals ที่ถูกลบ
            IndicatorUnitData(strategicObjective);
            IndicatorData(strategicObjective);
            strategicObjective.Goals = strategicObjective.Goals.Where(g => !g.isDeleteForm || g.ID != 0).ToList();
            ModelState.Clear();
            return View("Manage", strategicObjective);
        }

        private void FindFormDelete(ICollection<Goal> goals)
        {
            foreach (var g in goals.ToList()) // Use ToList() to create a copy of the collection
            {
                if (g.isDeleteForm)
                {
                    if (g.ID == 0)
                    {
                        goals.Remove(g);
                    }
                    else
                    {
                        g.isDelete = true;
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Information(StrategicObjective strategicObjective)
        {
            var lastgoal = db.Goals.OrderByDescending(g => g.No).FirstOrDefault()?.No ?? 0;

            // สร้าง Goal ใหม่
            Goal goal = new Goal()
            {
                StrategicObjectiveID = strategicObjective.ID,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                isDelete = false,
                isLastDelete = false,
                IsAddIndicator = false,
            };

            if (strategicObjective.Goals == null || !strategicObjective.Goals.Any())
            {
                // ถ้าไม่มี Goals ใน StrategicObjective นี้เลย
                goal.No = lastgoal + 1;
            }
            else
            {
                // ตรวจสอบว่ามี SOEPlanIndicators ใน Goal หรือไม่
                if (strategicObjective.Goals.Any(g => g.SOEPlanIndicators.Any()))
                {
                    // นับ No ต่อจาก No ของ SOEPlanIndicator ล่าสุด
                    var lastNo = strategicObjective.Goals.SelectMany(g => g.SOEPlanIndicators).Max(si => si.No);
                    goal.No = lastNo + 1;
                }
                else
                {
                    // หา No ของ Goal ล่าสุด
                    var lastGoal = strategicObjective.Goals.OrderByDescending(g => g.No).FirstOrDefault();
                    var lastGoalNo = lastGoal?.No ?? 0;

                    // นับ No ต่อจาก No ของ Goal ล่าสุด
                    goal.No = lastGoalNo + 1;
                }
            }

            if (strategicObjective.Goals == null)
            {
                strategicObjective.Goals = new List<Goal>();
            }

            strategicObjective.Goals.Add(goal);
        }

        public ActionResult UpdateData(StrategicObjective strategicObjective)
        {
            // อัพเดทข้อมูล Indicator และ IndicatorUnit
            IndicatorData(strategicObjective);
            IndicatorUnitData(strategicObjective);

            // ส่งข้อมูลที่ต้องการใน View
            return View("Manage", strategicObjective);
        }

        private void IndicatorData(StrategicObjective strategicObjective)
        {
            // วนลูปผ่าน Goals และ SOEPlanIndicators เพื่อกำหนดค่า IndicatorItem
            foreach (var item in strategicObjective.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicators)
                {
                    item2.IndicatorItem = db.Indicators
                        .Where(m => m.isDelete == false)
                        .Select(m => new SelectListItem() { Value = m.ID.ToString(), Text = m.Indicator1 });
                }
            }
        }

        private void IndicatorUnitData(StrategicObjective strategicObjective)
        {
            // วนลูปผ่าน Goals และ SOEPlanIndicators เพื่อกำหนดค่า IndicatorUnitItem
            foreach (var item in strategicObjective.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicators)
                {
                    if (item2.IndicatorID != null)
                    {
                        item2.IndicatorUnitItem = db.IndicatorUnits
                            .Where(m => m.IndicatorID == item2.IndicatorID && m.isDelete == false)
                            .Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Unit });
                    }
                }
            }
        }
    }
}
