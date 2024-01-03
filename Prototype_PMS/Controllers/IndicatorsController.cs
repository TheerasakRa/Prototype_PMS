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
    public class IndicatorsController : Controller
    {
        private PMSEntities1 db = new PMSEntities1();

        // GET: Indicators
        public ActionResult Index(string Division, string Indicator, int? IndicatorDetailStatusID)
        {
            
            if (Division == "") { Division = null; }
            if (Indicator == "") { Indicator = null; }
            var indicators = db.Indicators.Where(i => i.isDelete != true).Include(i => i.IndicatorDetailStatu);
            //var indicators = db.Indicators.Where(s => s.isDelete == false);

            if (Indicator != null)
            {
                indicators = indicators.Where(s => s.Indicator1.Contains(Indicator));
            }

            if (Division != null)
            {
                indicators = indicators.Where(s => s.IndicatorOwners.Any(q => q.Division == Division));
            }

            if (IndicatorDetailStatusID != null)
            {
                indicators = indicators.Where(s => s.IndicatorDetailStatusID == IndicatorDetailStatusID);
            }


            ViewBag.Division = indicatorOwners();
            ViewBag.Statuses = indicatorDetailStatuses();


            return View(indicators.ToList());
        }


        // GET: Indicators/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = db.Indicators.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            return View(indicator);
        }

        // GET: Indicators/Create
        public ActionResult Create()
        {
            Indicator indicator = new Indicator();

            var indicatorTypes = db.IndicatorTypes.ToList();
            foreach (var indicatorType in indicatorTypes)
            {
                IndicatorXIndicatorType indicatorXIndicatorType = new IndicatorXIndicatorType();
                indicatorXIndicatorType.IndicatorType = indicatorType;
                indicatorXIndicatorType.IndicatorTypeID = indicatorType.ID;
                indicator.IndicatorXIndicatorTypes.Add(indicatorXIndicatorType);
            }

            LoadCreateUnit(indicator);


            Session["IndicatorXIndicatorTypes"] = new List<IndicatorXIndicatorType>(indicator.IndicatorXIndicatorTypes);

            ViewBag.Division = indicatorOwners();
            ViewBag.Statuses = indicatorDetailStatuses();

            return View(indicator);
        }


        // POST: Indicators/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Indicator indicator)
        {
            indicator.CreateDate = DateTime.Now;
            indicator.UpdateDate = DateTime.Now;
            indicator.isDelete = false;
            indicator.isLastDelete = false;
            var x = indicator.IndicatorXIndicatorTypes.ToList();
            indicator.IndicatorXIndicatorTypes = null;
            db.Indicators.Add(indicator);
            foreach (var temp in x)
            {
                temp.CreateDate = DateTime.Now;
                temp.UpdateDate = DateTime.Now;
                temp.IndicatorType = null;
                temp.IndicatorID = indicator.ID;
                db.IndicatorXIndicatorTypes.Add(temp);
                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }
        // GET: Indicators/Edit/5
        public ActionResult Edit(int? id)
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Indicator indicator = db.Indicators.Find(id);

            if (indicator == null)
            {
                return HttpNotFound();
            }

            Session["IndicatorXIndicatorTypes"] = new List<IndicatorXIndicatorType>(indicator.IndicatorXIndicatorTypes);

            ViewBag.Division = indicatorOwners();
            ViewBag.Statuses = indicatorDetailStatuses();
            var x = indicator.IndicatorOwners.ToList();
            return View(indicator);
        }

        public void KeepTypeName(Indicator indicator)
        {
            ModelState.Clear();
            var sessionIndicatorXIndicatorTypes = Session["IndicatorXIndicatorTypes"] as List<IndicatorXIndicatorType>;
            if (sessionIndicatorXIndicatorTypes != null)
            {
                indicator.IndicatorXIndicatorTypes = sessionIndicatorXIndicatorTypes;
            }
        }


        // POST: Indicators/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Indicator indicator)
        {
            var Owner = indicator.IndicatorOwners.ToList();
            var Unit = indicator.IndicatorUnits.ToList();
            var Type = indicator.IndicatorXIndicatorTypes.ToList();
            indicator.IndicatorOwners = null;
            indicator.IndicatorUnits = null;
            indicator.IndicatorXIndicatorTypes = null;
            indicator.UpdateDate = DateTime.Now;
            db.Entry(indicator).State = EntityState.Modified;
            foreach (var OwnerItem in Owner)
            {
                if (OwnerItem.ID == 0)
                {
                    OwnerItem.IndicatorID = indicator.ID;
                    db.IndicatorOwners.Add(OwnerItem);
                    db.SaveChanges();
                }
                else
                {
                    OwnerItem.UpdateDate = DateTime.Now;
                    db.Entry(OwnerItem).State = EntityState.Modified;
                }
            }
            foreach (var UnitItem in Unit)
            {
                if (UnitItem.ID == 0)
                {
                    UnitItem.IndicatorID = indicator.ID;
                    db.IndicatorUnits.Add(UnitItem);
                    db.SaveChanges();
                }
                else
                {
                    UnitItem.UpdateDate = DateTime.Now;
                    db.Entry(UnitItem).State = EntityState.Modified;
                }
            }
            foreach (var TypeItem in Type)
            {
                if (TypeItem.ID == 0)
                {

                    TypeItem.IndicatorType = null;
                    TypeItem.IndicatorID = indicator.ID;
                    db.IndicatorXIndicatorTypes.Add(TypeItem);
                    db.SaveChanges();
                }
                else
                {
                    TypeItem.UpdateDate = DateTime.Now;
                    TypeItem.IndicatorType = null;
                    db.Entry(TypeItem).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        // GET: Indicators/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = db.Indicators.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            return View(indicator);
        }


        // POST: Indicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Indicator indicator = db.Indicators.Find(id);
            //db.Indicators.Remove(indicator);
            indicator.isDelete = true;
            db.SaveChanges();
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            ViewBag.DivisionBag = indicatorOwners();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddOwner(Indicator indicator)
        {
            IndicatorOwner indicatorOwner = new IndicatorOwner();
            indicatorOwner.CreateDate = DateTime.Now;
            indicatorOwner.UpdateDate = DateTime.Now;
            indicatorOwner.isDelete = false;

            indicator.IndicatorOwners.Add(indicatorOwner);
            ViewBag.Statuses = indicatorDetailStatuses();
            ViewBag.Division = indicatorOwners();
            return View("Create", indicator);
        }


        [HttpPost]
        public ActionResult AddUnit(Indicator indicator)
        {

            IndicatorUnit indicatorUnit = new IndicatorUnit();
            indicatorUnit.CreateDate = DateTime.Now;
            indicatorUnit.UpdateDate = DateTime.Now;
            indicatorUnit.isDelete = false;
            indicator.IndicatorUnits.Add(indicatorUnit);

            ViewBag.Statuses = indicatorDetailStatuses();
            ViewBag.Division = indicatorOwners();
            //LoadCreateUnit(indicator);

            return View("Create", indicator);
        }
        [HttpPost]
        public ActionResult EditAddOwner(Indicator indicator)
        {
            IndicatorOwner indicatorOwner = new IndicatorOwner();
            indicatorOwner.CreateDate = DateTime.Now;
            indicatorOwner.UpdateDate = DateTime.Now;
            indicatorOwner.isDelete = false;

            indicator.IndicatorOwners.Add(indicatorOwner);
            ViewBag.Statuses = indicatorDetailStatuses();
            ViewBag.Division = indicatorOwners();
            return View("Edit", indicator);
        }


        [HttpPost]
        public ActionResult EditAddUnit(Indicator indicator)
        {

            IndicatorUnit indicatorUnit = new IndicatorUnit();
            indicatorUnit.CreateDate = DateTime.Now;
            indicatorUnit.UpdateDate = DateTime.Now;
            indicatorUnit.isDelete = false;
            indicator.IndicatorUnits.Add(indicatorUnit);

            ViewBag.Statuses = indicatorDetailStatuses();
            ViewBag.Division = indicatorOwners();
            //LoadCreateUnit(indicator);

            return View("Edit", indicator);
        }
        public SelectList indicatorOwners()
        {
            var uniqueOwners = db.Owners
                .Select(i => new SelectListItem
                {
                    Text = i.Division,
                    Value = i.Division
                })
                .ToList();

            SelectList selectListOwner = new SelectList(uniqueOwners, "Value", "Text");

            return selectListOwner;
        }
        public SelectList indicatorDetailStatuses()
        {
            var uniqueStatuses = db.IndicatorDetailStatus
                .Select(a => new SelectListItem
                {
                    Text = a.Status,
                    Value = a.ID.ToString()
                })
                .Distinct()  // กรองค่าที่ไม่ซ้ำกัน
                .ToList();

            SelectList selectListStatus = new SelectList(uniqueStatuses, "Value", "Text");

            return selectListStatus;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void LoadCreateUnit(Indicator indicator)
        {
            ViewBag.Statuses = indicatorDetailStatuses();
            ViewBag.Division = indicatorOwners();
            IndicatorUnit indicatorUnit = new IndicatorUnit();
            indicator.IndicatorUnits.Add(indicatorUnit);
        }

        [HttpPost]
        public ActionResult OwnerDelete(Indicator indicator)
        {
            ViewBag.Statuses = indicatorDetailStatuses();
            indicator.IndicatorOwners = FindFormDelete(indicator.IndicatorOwners);
            ViewBag.Division = indicatorOwners();
            return View("Create", indicator);
        }
        private ICollection<IndicatorOwner> FindFormDelete(ICollection<IndicatorOwner> indicatorOwners)
        {
            foreach (var d in indicatorOwners)
            {
                if (d.isOwnerDelete == true)
                {
                    if (d.ID == 0)
                    {
                        var io = indicatorOwners.ToList();
                        io.RemoveAt(io.IndexOf(d));
                        indicatorOwners = io;
                    }

                    else
                    {
                        d.isDelete = true;
                    }
                }
            }
            return indicatorOwners;
        }
        [HttpPost]
        public ActionResult UnitDelete(Indicator indicator)
        {
            ModelState.Clear();
            indicator.IndicatorUnits = FindUnitDelete(indicator.IndicatorUnits);
            ViewBag.Division = indicatorOwners();
            ViewBag.Statuses = indicatorDetailStatuses();
            return View("Create", indicator);
        }
        private ICollection<IndicatorUnit> FindUnitDelete(ICollection<IndicatorUnit> indicatorUnits)
        {

            foreach (var d in indicatorUnits)
            {
                if (d.isUnitDelete == true)
                {
                    if (d.ID == 0)
                    {
                        var io = indicatorUnits.ToList();
                        io.RemoveAt(io.IndexOf(d));
                        indicatorUnits = io;
                    }

                    else
                    {
                        d.isDelete = true;
                    }
                }
            }
            return indicatorUnits;
        }
        [HttpPost]
        public ActionResult EditOwnerDelete(Indicator indicator)
        {
            ViewBag.Statuses = indicatorDetailStatuses();
            indicator.IndicatorOwners = EditFindFormDelete(indicator.IndicatorOwners);
            ViewBag.Division = indicatorOwners();
            return View("Edit", indicator);
        }

        private ICollection<IndicatorOwner> EditFindFormDelete(ICollection<IndicatorOwner> indicatorOwners)
        {
            ModelState.Clear();
            var ownersToDelete = new List<IndicatorOwner>();

            foreach (var d in indicatorOwners)
            {
                if (d.isOwnerDelete == true)
                {
                    if (d.ID == 0)
                    {
                        ownersToDelete.Add(d);
                    }
                    else
                    {
                        d.isDelete = true;
                    }
                }
            }

            // Remove the unsaved owners from the original list
            foreach (var ownerToDelete in ownersToDelete)
            {
                indicatorOwners.Remove(ownerToDelete);
            }

            return indicatorOwners;
        }
        [HttpPost]
        public ActionResult EditUnitDelete(Indicator indicator)
        {
            ModelState.Clear();
            indicator.IndicatorUnits = FindUnitDelete(indicator.IndicatorUnits);
            ViewBag.Division = indicatorOwners();
            ViewBag.Statuses = indicatorDetailStatuses();
            return View("Edit", indicator);
        }
       
        public ActionResult RecycleBin(String Division, string Indicator)
        {
            var indicators = db.Indicators.Where(s => s.isDelete == true && s.isLastDelete == false);

            if (Division == "") { Division = null; }
            if (Indicator == "") { Indicator = null; }
            if (Indicator != null)
            {
                indicators = indicators.Where(s => s.Indicator1.Contains(Indicator));
            }
            if (Division != null)
            {
                indicators = indicators.Where(s => s.IndicatorOwners.Where(q => q.Division == Division).Count() > 0);
            }
            ViewBag.DivisionBag = indicatorOwners();
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            return View("RecycleBin", indicators);
        }
        public ActionResult Revert(int id)
        {
            var indicators = db.Indicators.Find(id);
            indicators.isDelete = false;
            db.Entry(indicators).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }
        public ActionResult DeleteLast(int id)
        {
            var indicators = db.Indicators.Find(id);
            indicators.isLastDelete = true;
            db.Entry(indicators).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }
    }
}
