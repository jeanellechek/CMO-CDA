using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMO101.Models;

namespace CMO101.Controllers
{
    public class crisisLevelsController : Controller
    {
        private cmoAzure db = new cmoAzure();

        // GET: crisisLevels
        public ActionResult Index()
        {
            return View(db.crisisLevels.ToList());
        }

        // GET: crisisLevels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            crisisLevel crisisLevel = db.crisisLevels.Find(id);
            if (crisisLevel == null)
            {
                return HttpNotFound();
            }
            return View(crisisLevel);
        }

        // GET: crisisLevels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: crisisLevels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "crisisID,crisisLevel1")] crisisLevel crisisLevel)
        {
            if (ModelState.IsValid)
            {
                db.crisisLevels.Add(crisisLevel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(crisisLevel);
        }

        // GET: crisisLevels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            crisisLevel crisisLevel = db.crisisLevels.Find(id);
            if (crisisLevel == null)
            {
                return HttpNotFound();
            }
            return View(crisisLevel);
        }

        // POST: crisisLevels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "crisisID,crisisLevel1")] crisisLevel crisisLevel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(crisisLevel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(crisisLevel);
        }

        // GET: crisisLevels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            crisisLevel crisisLevel = db.crisisLevels.Find(id);
            if (crisisLevel == null)
            {
                return HttpNotFound();
            }
            return View(crisisLevel);
        }

        // POST: crisisLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            crisisLevel crisisLevel = db.crisisLevels.Find(id);
            db.crisisLevels.Remove(crisisLevel);
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
