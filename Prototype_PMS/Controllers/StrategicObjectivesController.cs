using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;
using Prototype_PMS.Models;

namespace Prototype_PMS.Controllers
{
    public class StrategicObjectivesController : Controller
    {
        private PMSEntities1 db = new PMSEntities1();

        // GET: StrategicObjectives
        public ActionResult Index(int? SOEPlanID, string StrategicObjectives)
        {

            SOEPlan sOEPlan = db.SOEPlans.Find(SOEPlanID);

            ViewBag.SOEPlansYear = sOEPlan.StartEndYear;
            ViewBag.SOEPlansCreateBy = sOEPlan.CreateBy;
            ViewBag.SOEPlansID = SOEPlanID;

            sOEPlan.StrategicObjectives = sOEPlan.StrategicObjectives.Where(s => s.isDelete == false).ToList(); 

            List<StrategicObjective> strategicObjectives = db.StrategicObjectives.Where(s => s.SOEPlanID == SOEPlanID).ToList();

            if (!String.IsNullOrEmpty(StrategicObjectives))
            {
                strategicObjectives = strategicObjectives.Where(s => s.StrategicObjective1.Contains(StrategicObjectives)).ToList();
            }

            return View(strategicObjectives);
        }

        // GET: StrategicObjectives/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
            if (strategicObjective == null)
            {
                return HttpNotFound();
            }
            return View(strategicObjective);
        }

        // GET: StrategicObjectives/Create
        public ActionResult Create(int? SOEPlanID)
        {
            ViewBag.SOEPlansID = SOEPlanID;
            return View();
        }

        // POST: StrategicObjectives/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StrategicObjective strategicObjective, int? SOEPlanID)
        {
            if (ModelState.IsValid)
            {
                strategicObjective.UpdateDate = DateTime.Now;
                strategicObjective.CreateDate = DateTime.Now;
                strategicObjective.isLastDelete = false;
                strategicObjective.isDelete = false;
                strategicObjective.SOEPlanID = SOEPlanID;
                db.StrategicObjectives.Add(strategicObjective);
                db.SaveChanges();

            }

            return RedirectToAction("Index", new { SOEPlanID = SOEPlanID });
        }

        // GET: StrategicObjectives/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
            if (strategicObjective == null)
            {
                return HttpNotFound();
            }
            ViewBag.SEOPlanID = new SelectList(db.SOEPlans, "ID", "ID", strategicObjective.SOEPlanID);
            return View(strategicObjective);
        }

        // POST: StrategicObjectives/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( StrategicObjective strategicObjective)
        {
            if (ModelState.IsValid)
            {
                db.Entry(strategicObjective).State = EntityState.Modified;
                db.SaveChanges();

                if (strategicObjective.SOEPlanID != null)
                {
                    return RedirectToAction("Index", new { SOEPlanID = strategicObjective.SOEPlanID });
                }
                return RedirectToAction("Index");
            }

            //ViewBag.SEOPlanID = new SelectList(db.SOEPlans, "ID", "ID", strategicObjective.SOEPlanID);
            return View(strategicObjective);
        }

        // GET: StrategicObjectives/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
            if (strategicObjective == null)
            {
                return HttpNotFound();
            }

            strategicObjective.isDelete = true;
            db.SaveChanges();

            return RedirectToAction("Index", new { SOEPlanID = strategicObjective.SOEPlanID });
        }

        // POST: StrategicObjectives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id , int? SOEPlanID)
        {
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
            strategicObjective.isDelete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        public ActionResult RecycleBin(int? SOEPlanID)
        {
            if (SOEPlanID != null)
            {
                SOEPlan sOEPlan = db.SOEPlans.Find(SOEPlanID);
                {
                    if (sOEPlan.isDelete == true)
                    {
                        return RedirectToAction("Index",new {SOEPlanID = SOEPlanID});
                    }
                    else { 
                        var strategicObjectives = db.StrategicObjectives.Where(m => m.isDelete == true && m.isLastDelete == false).ToList();
                        return View(strategicObjectives);
                    }
                }
            }
            return View();
        }


        [HttpGet]
        public ActionResult Recover(int? id)
        {
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
            if (strategicObjective != null)
            {
                strategicObjective.UpdateDate = DateTime.Now;
                strategicObjective.isDelete = false;
                db.SaveChanges();
            }

            return RedirectToAction("RecycleBin",  new { SOEPlanID = strategicObjective.SOEPlanID});
        }

        [HttpGet]
        public ActionResult LastDelete(int? id)
        {
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
            if (strategicObjective != null)
            {
                strategicObjective.UpdateDate = DateTime.Now;
                strategicObjective.isLastDelete = true;
                db.SaveChanges();
            }

            return RedirectToAction("RecycleBin", new { SOEPlanID = strategicObjective.SOEPlanID } );
        }
    }

}

