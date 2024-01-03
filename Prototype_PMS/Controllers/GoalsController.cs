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

        // GET: Goals
        public ActionResult Manage(int? StrategicObjectiveID)
        {
            // ดึงข้อมูล Indicator ที่ยังไม่ถูกลบ
            var indicators = db.Indicators.Where(s => s.isDelete == false);

            // ค้นหา StrategicObjective จาก ID
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(StrategicObjectiveID);

            // ถ้าไม่พบ StrategicObjective ให้แสดง HTTP 404 Not Found
            if (strategicObjective == null)
            {
                return HttpNotFound();
            }

            // ถ้า StrategicObjective ไม่มี Goals ให้เพิ่มข้อมูล Goal ใหม่
            if (strategicObjective.Goals.Count == 0)
            {
                Information(strategicObjective);
            }

            // ส่งข้อมูลที่ต้องการใน View
            ViewBag.Strantegic = strategicObjective.StrategicObjective1;
            ViewBag.StrategicObjectiveID = StrategicObjectiveID;
            IndicatorUnitData(strategicObjective);
            IndicatorData(strategicObjective);
            return View(strategicObjective);
        }

        [HttpPost]
        public ActionResult AddGoal(StrategicObjective strategicObjective)
        {
            // เพิ่มข้อมูล Goal
            Information(strategicObjective);

            // อัพเดทข้อมูล IndicatorUnit และ Indicator
            IndicatorUnitData(strategicObjective);
            IndicatorData(strategicObjective);

            ViewBag.Strantegic = strategicObjective.StrategicObjective1;

            // ส่งข้อมูลที่ต้องการใน View
            ModelState.Clear();
            return View("Manage", strategicObjective);
        }

        [HttpPost]
        public ActionResult AddIndicator(StrategicObjective strategicObjective)
        {
            // ลูปผ่าน Goals และเพิ่ม Indicator ใหม่ในแต่ละ Goal ตามที่ผู้ใช้เลือก
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

                    // กำหนด No ให้ SOEPlanIndicator
                    var last = db.SOEPlanIndicators.ToList().LastOrDefault();
                    if (last == null)
                    {
                        last = new SOEPlanIndicator();
                        last.No = 0;
                    }
                    SOEPlanIndicator.No = last.No + 1;

                    // เพิ่ม SOEPlanIndicator ใน Goal
                    goal.SOEPlanIndicators.Add(SOEPlanIndicator);
                    goal.IsAddIndicator = false;
                }
            }

            ViewBag.Strantegic = strategicObjective.StrategicObjective1;
            // ส่งข้อมูลที่ต้องการใน View
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

                        // ล้าง SOEPlanIndicators ใน Goal
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
            var goal = strategicObjective.Goals;
            // วนลูปผ่าน Goals และบันทึกหรืออัพเดทข้อมูล

            SaveDataSOEGoal(goal);
            // บันทึกข้อมูลลงในฐานข้อมูล
            db.SaveChanges();

            // Redirect ไปที่ Action อื่น
            return RedirectToAction("Index", "StrategicObjectives", new { SOEPlanID = strategicObjective.SOEPlanID });
        }

        private void SaveDataSOEGoal(ICollection<Goal> goal)
        {
            foreach (var a in goal)
            {

                var SOEPlanIndicator = a.SOEPlanIndicators;
                a.SOEPlanIndicators = new List<SOEPlanIndicator>();
                a.UpdateDate = DateTime.Now;
                if (a.ID == 0)
                {
                    if (a.isDelete != true)
                    {
                        db.Goals.Add(a);
                    }
                }
                else
                {
                    db.Entry(a).State = EntityState.Modified;

                }

                foreach (var b in SOEPlanIndicator)
                {
                    if (b.ID == 0)
                    {
                        if (b.IsDelete != true)
                        {
                            b.GoalID = a.ID;
                            db.SOEPlanIndicators.Add(b);
                        }
                    }
                    else
                    {
                        db.Entry(b).State = EntityState.Modified;
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
            // ล้าง ModelState
            ModelState.Clear();

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

            // กำหนด No ให้ Goal
            if (db != null)
            {
                var last = db.Goals.ToList().LastOrDefault();
                goal.No = goal.No + 1;
                if (last == null)
                {
                    goal.No = null; 
                }
                else
                {
                    goal.No = last.No + 1;
                }
            }



            // เพิ่ม Goal ใน StrategicObjective
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
