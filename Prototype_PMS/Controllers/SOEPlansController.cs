using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Prototype_PMS.Models;

namespace Prototype_PMS.Controllers
{
    public class SOEPlansController : Controller
    {
        private PMSEntities1 db = new PMSEntities1();

        // GET: SOEPlans
        public ActionResult Index()
        {
            var c = db.SOEPlans.Where(m => !(bool)m.isDelete);
            return View(c);
        }

        // GET: SOEPlans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SOEPlan sOEPlan = db.SOEPlans.Find(id);
            if (sOEPlan == null)
            {
                return HttpNotFound();
            }
            return View(sOEPlan);
        }

        // GET: SOEPlans/Create
        public ActionResult Create()
        {
            ViewBag.StartYearList = GetYearList();
            ViewBag.EndYearList = GetYearList();
            return View();
        }

        // POST: SOEPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SOEPlan sOEPlan)
        {
            if (ModelState.IsValid)
            {
                Random random = new Random();

                int randomUserId = random.Next();

                sOEPlan.CreateBy = randomUserId;
                sOEPlan.UpdateDate = DateTime.Now;
                sOEPlan.CreateDate = DateTime.Now;
                sOEPlan.isLastDelete = false;
                sOEPlan.isDelete = false;

                ViewBag.StartYearList = GetYearList();
                ViewBag.EndYearList = GetYearList();

                db.SOEPlans.Add(sOEPlan);
                db.SaveChanges();
                
            }

            return RedirectToAction("Index");
        }

        private IEnumerable<SelectListItem> GetYearList()
        {
            var yearList = Enumerable.Range(DateTime.Now.Year - 10, 20)
                .Select(x => new SelectListItem
                {
                    Text = x.ToString(),
                    Value = x.ToString(),
                    Selected = x == DateTime.Now.Year
                });
            var emptyItem = new SelectListItem { Text = "Selected", Value = "", Selected = false };
            return new[] { emptyItem }.Concat(yearList);
        }
        // GET: SOEPlans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SOEPlan sOEPlan = db.SOEPlans.Find(id);
            ViewBag.StartYearList = GetYearList();
            ViewBag.EndYearList = GetYearList();
            if (sOEPlan == null)
            {
                return HttpNotFound();
            }
            return View(sOEPlan);
        }

        // POST: SOEPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,StartYear,EndYear,CreateBy,UpdateBy,CreateDate,UpdateDate,isDelete,isLastDelete")] SOEPlan sOEPlan)
        {
            if (ModelState.IsValid)
            {
                sOEPlan.UpdateDate = DateTime.Now;
                sOEPlan.CreateDate = DateTime.Now;
                sOEPlan.isLastDelete = false;
                sOEPlan.isDelete = false;
                ViewBag.StartYearList = GetYearList();
                ViewBag.EndYearList = GetYearList();
                db.Entry(sOEPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sOEPlan);
        }

        // GET: SOEPlans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SOEPlan sOEPlan = db.SOEPlans.Find(id);
            if (sOEPlan == null)
            {
                return HttpNotFound();
            }

            sOEPlan.isDelete = true;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: SOEPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SOEPlan sOEPlan = db.SOEPlans.Find(id);
            sOEPlan.isDelete = true;
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
        public ActionResult RecycleBin()
        {
            var c = db.SOEPlans.Where(m => m.isDelete == true && m.isLastDelete == false).ToList();
            return View(c);
        }

        [HttpGet]
        public ActionResult Recover(int? id)
        {
            SOEPlan sOEPlan = db.SOEPlans.Find(id);
            sOEPlan.UpdateDate = DateTime.Now;
            sOEPlan.isDelete = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult LastDelete(int? id)
        {
            SOEPlan sOEPlan = db.SOEPlans.Find(id);
            sOEPlan.UpdateDate = DateTime.Now;
            sOEPlan.isLastDelete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
