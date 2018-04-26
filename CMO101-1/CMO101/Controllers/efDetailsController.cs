using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMO101.Models;

namespace CMO101.Controllers
{
    public class efDetailsController : Controller
    {
        private cmoAzure db = new cmoAzure();

        // GET: efDetails
        public async Task<ActionResult> Index()
        {
            return View(await db.efDetails.ToListAsync());
        }

        // GET: efDetails/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            efDetail efDetail = await db.efDetails.FindAsync(id);
            if (efDetail == null)
            {
                return HttpNotFound();
            }
            return View(efDetail);
        }

        // GET: efDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: efDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "caseID,dateTime,action")] efDetail efDetail)
        {
            if (ModelState.IsValid)
            {
                db.efDetails.Add(efDetail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(efDetail);
        }

        // GET: efDetails/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            efDetail efDetail = await db.efDetails.FindAsync(id);
            if (efDetail == null)
            {
                return HttpNotFound();
            }
            return View(efDetail);
        }

        // POST: efDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "caseID,dateTime,action")] efDetail efDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(efDetail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(efDetail);
        }

        // GET: efDetails/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            efDetail efDetail = await db.efDetails.FindAsync(id);
            if (efDetail == null)
            {
                return HttpNotFound();
            }
            return View(efDetail);
        }

        // POST: efDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            efDetail efDetail = await db.efDetails.FindAsync(id);
            db.efDetails.Remove(efDetail);
            await db.SaveChangesAsync();
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
