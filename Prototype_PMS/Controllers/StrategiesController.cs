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
    public class StrategiesController : Controller
    {
        private PMSEntities1 db = new PMSEntities1();

        // GET: Strategies
        public ActionResult Index(int? StrategicObjectiveID)
        {
            if (StrategicObjectiveID == null)
            {
                return RedirectToAction("Error");
            }

            StrategicObjective strategicObjective = db.StrategicObjectives.Find(StrategicObjectiveID);
            strategicObjective.Strategies = strategicObjective.Strategies.Where(s => s.isDelete == false).ToList();

            if (strategicObjective == null)
            {
                return HttpNotFound();
            }

            ViewBag.TitleStg = strategicObjective.StrategicObjective1;
            ViewBag.StrategicObjectiveID = StrategicObjectiveID;


            return View(strategicObjective.Strategies.ToList());
        }

        // GET: Strategies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = db.Strategies.Find(id);
            if (strategy == null)
            {
                return HttpNotFound();
            }
            return View(strategy);
        }

        // GET: Strategies/Create
        public ActionResult Create(int? StrategicObjectiveID)
        {
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(StrategicObjectiveID);

            ViewBag.StObID = strategicObjective.ID;

            var stategy = new Strategy();
            stategy.CreateDate = DateTime.Now;
            stategy.UpdateDate = DateTime.Now;
            stategy.isDelete = false;
            stategy.isLastDelete = false;
            stategy.StrategicObjectiveID = StrategicObjectiveID;
            return View(stategy);
        }
        [HttpPost]
        public ActionResult Addtactic(Strategy strategy)
        {
            ModelState.Clear();

            Tactic tactic = new Tactic();
            tactic.StrategyID = strategy.ID;
            tactic.isDelete = false;
            tactic.isLastDelete = false;

            if (strategy.Tactics.Any())
            {
                // หาค่า No ที่มากที่สุดใน db
                var lastNo = strategy.Tactics.Max(si => si.No);

                // เพิ่มค่า No ของ Tactic ให้เป็นค่าที่มากที่สุดบนฐานข้อมูลแล้วบวก 1
                tactic.No = lastNo + 1;
            }
            else
            {
                tactic.No = 1;
            }
            tactic.StrategyID = strategy.ID;
            strategy.Tactics.Add(tactic);
            return View("Create", strategy);
        }

        public ActionResult AddtacticEdit(Strategy strategy)
        {
            ModelState.Clear();

            Tactic tactic = new Tactic();
            tactic.StrategyID = strategy.ID;
            tactic.isDelete = false;
            tactic.isLastDelete = false;
            if (strategy.Tactics.Any())
            {
                // หาค่า No ที่มากที่สุดใน db
                var lastNo = strategy.Tactics.Max(si => si.No);

                // เพิ่มค่า No ของ Tactic ให้เป็นค่าที่มากที่สุดบนฐานข้อมูลแล้วบวก 1
                tactic.No = lastNo + 1;
            }
            else
            {
                tactic.No = 1;
            }
            tactic.StrategyID = strategy.ID;

            strategy.Tactics.Add(tactic);
            return View("Edit", strategy);
        }


        // POST: Strategies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Strategy strategy)
        {
            if (ModelState.IsValid)
            {
                strategy.CreateDate = DateTime.Now;
                strategy.UpdateDate = DateTime.Now;
                strategy.isDelete = false;
                strategy.isLastDelete = false;
                var laststategy = db.Strategies.LongCount();
                strategy.No = (int)laststategy + 1;

                foreach (var tactic in strategy.Tactics)
                {
                    tactic.StrategyID = strategy.ID;
                    tactic.CreateDate = DateTime.Now;
                    tactic.UpdateDate = DateTime.Now;
                    tactic.isDelete = false;
                    tactic.isLastDelete = false;

                    tactic.Strategy = strategy;

                    db.Tactics.Add(tactic);
                }

                db.Strategies.Add(strategy);
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { StrategicObjectiveID = strategy.StrategicObjectiveID });
        }



        // GET: Strategies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = db.Strategies.Find(id);
            if (strategy == null)
            {
                return HttpNotFound();
            }
            strategy.Tactics = strategy.Tactics.Where(m => m.isDelete != true).ToList();
            return View(strategy);
        }

        // POST: Strategies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Strategy strategy)
        {
            if (ModelState.IsValid)
            {
                strategy.UpdateDate = DateTime.Now;
                strategy.isDelete = false;
                strategy.isLastDelete = false;

                foreach (var tactic in strategy.Tactics)
                {
                    if (tactic.ID != 0)
                    {
                        db.Entry(tactic).State = EntityState.Modified;
                        tactic.UpdateDate = DateTime.Now;
                    }
                    else
                    {
                        tactic.StrategyID = strategy.ID;
                        tactic.CreateDate = DateTime.Now;
                        tactic.UpdateDate = DateTime.Now;
                        tactic.isDelete = false;
                        tactic.isLastDelete = false;
                        db.Tactics.Add(tactic);
                    }
                }
                db.Entry(strategy).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { StrategicObjectiveID = strategy.StrategicObjectiveID });
        }

        // GET: Strategies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = db.Strategies.Find(id);
            if (strategy == null)
            {
                return HttpNotFound();
            }

            strategy.isDelete = true;
            db.SaveChanges();

            return RedirectToAction("Index", new { StrategicObjectiveID = strategy.StrategicObjectiveID });
        }

        // POST: Strategies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? StrategicObjectiveID)
        {
            Strategy strategy = db.Strategies.Find(id);
            strategy.isDelete = true;
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
        public ActionResult RecycleBin(int? StrategicObjectiveID)
        {
            if (StrategicObjectiveID != null)
            {
                StrategicObjective strategicObjective = db.StrategicObjectives.Find(StrategicObjectiveID);
                if (strategicObjective != null)
                {
                    if (strategicObjective.isDelete == true)
                    {
                        return RedirectToAction("Index", new { StrategicObjectiveID = StrategicObjectiveID });
                    }
                    else
                    {
                        var strategics = db.Strategies.Where(m => m.isDelete == true && m.isLastDelete == false).ToList();

                        return View(strategics);
                    }
                }
            }
            return View(new List<Strategy>());
        }


        [HttpGet]
        public ActionResult Recover(int? id)
        {
            Strategy strategy = db.Strategies.Find(id);
            if (strategy != null)
            {
                strategy.UpdateDate = DateTime.Now;
                strategy.isDelete = false;
                db.SaveChanges();
            }

            return RedirectToAction("RecycleBin", new { StrategicObjectiveID = strategy.StrategicObjectiveID });
        }

        [HttpGet]
        public ActionResult LastDelete(int? id)
        {
            Strategy strategy = db.Strategies.Find(id);
            if (strategy != null)
            {
                strategy.UpdateDate = DateTime.Now;
                strategy.isDelete = false;
                db.SaveChanges();
            }

            return RedirectToAction("RecycleBin", new { StrategicObjectiveID = strategy.StrategicObjectiveID });

        }

        [HttpPost, ActionName("DeleteFormTactic")]
        public ActionResult DeleteFormTactic(Strategy strategy)
        {
            FindFormDelete(strategy.Tactics);
            ModelState.Clear();
            return View("Create", strategy);
        }
        private void FindFormDelete(ICollection<Tactic> tactics)
        {
            foreach (var d in tactics)
            {
                ModelState.Clear();

                if (d.isDeleteFormTactic == true)
                {
                    d.isDelete = true;
                }
            }
        }
        [HttpPost, ActionName("DeleteFormTacticEdit")]
        public ActionResult DeleteFormTacticEdit(Strategy strategy)
        {
            FindFormDeleteEdit(strategy.Tactics);
            return View("Edit", strategy);
        }
        private void FindFormDeleteEdit(ICollection<Tactic> tactics)
        {
            foreach (var d in tactics)
            {
                ModelState.Clear();

                if (d.isDeleteFormTactic == true)
                {
                    d.isDelete = true;
                }
            }
        }
    }
}
