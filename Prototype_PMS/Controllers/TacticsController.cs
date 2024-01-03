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
    public class TacticsController : Controller
    {
        private PMSEntities1 db = new PMSEntities1();

        // GET: Tactics
        public ActionResult Index()
        {
            var tactics = db.Tactics.Include(t => t.Strategy);
            return View(tactics.ToList());
        }

        // GET: Tactics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tactic tactic = db.Tactics.Find(id);
            if (tactic == null)
            {
                return HttpNotFound();
            }
            return View(tactic);
        }

        // GET: Tactics/Create
        public ActionResult Create()
        {
            ViewBag.StrategyID = new SelectList(db.Strategies, "ID", "Strategy1");
            return View();
        }

        // POST: Tactics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,No,Tactic1,StrategyID,CreateBy,UpdateBy,CreateDate,UpdateDate,isDelete,isLastDelete")] Tactic tactic)
        {
            if (ModelState.IsValid)
            {
                db.Tactics.Add(tactic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StrategyID = new SelectList(db.Strategies, "ID", "Strategy1", tactic.StrategyID);
            return View(tactic);
        }

        // GET: Tactics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tactic tactic = db.Tactics.Find(id);
            if (tactic == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategyID = new SelectList(db.Strategies, "ID", "Strategy1", tactic.StrategyID);
            return View(tactic);
        }

        // POST: Tactics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,No,Tactic1,StrategyID,CreateBy,UpdateBy,CreateDate,UpdateDate,isDelete,isLastDelete")] Tactic tactic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tactic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StrategyID = new SelectList(db.Strategies, "ID", "Strategy1", tactic.StrategyID);
            return View(tactic);
        }

        // GET: Tactics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tactic tactic = db.Tactics.Find(id);
            if (tactic == null)
            {
                return HttpNotFound();
            }
            return View(tactic);
        }

        // POST: Tactics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tactic tactic = db.Tactics.Find(id);
            db.Tactics.Remove(tactic);
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
    }
}
