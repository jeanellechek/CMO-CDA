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
    public class userDetailsController : Controller
    {
        private cmoAzure db = new cmoAzure();

        // GET: userDetails
        public ActionResult Index()
        {
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (!Session["rank"].Equals("Admin"))
                return RedirectToAction("permission", "Home");

            return View(db.userDetails.ToList());
        }

        // GET: userDetails/Details/5
        public ActionResult Details(string id)
        {
            
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (!Session["rank"].Equals("Admin"))
                return RedirectToAction("permission", "Home");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userDetail userDetail = db.userDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            return View(userDetail);
        }

        // GET: userDetails/Create
        public ActionResult Create()
        {
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (!Session["rank"].Equals("Admin"))
                return RedirectToAction("permission", "Home");

            return View();
        }

        // POST: userDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "email,password,name,rank")] userDetail userDetail)
        {
            Session["firstAccess"] = 1;
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (!Session["rank"].Equals("Admin"))
                return RedirectToAction("permission", "Home");

            if (ModelState.IsValid)
            {
                db.userDetails.Add(userDetail);
                db.SaveChanges();
                Session["success"] = 1;
                return RedirectToAction("Index");
            }

            return View(userDetail);
        }

        // GET: userDetails/Edit/5
        public ActionResult Edit(string id)
        {
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (!Session["rank"].Equals("Admin"))
                return RedirectToAction("permission", "Home");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userDetail userDetail = db.userDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            return View(userDetail);
        }

        // POST: userDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "email,password,name,rank")] userDetail userDetail)
        {
            Session["firstAccess"] = 1;
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (!Session["rank"].Equals("Admin"))
                return RedirectToAction("permission", "Home");

            if (ModelState.IsValid)
            {
                db.Entry(userDetail).State = EntityState.Modified;
                db.SaveChanges();
                Session["success"] = 1;
                return RedirectToAction("Index");
            }
            return View(userDetail);
        }

        // GET: userDetails/Delete/5
        public ActionResult Delete(string id)
        {
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (!Session["rank"].Equals("Admin"))

                return RedirectToAction("permission", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userDetail userDetail = db.userDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            return View(userDetail);
        }

        // POST: userDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Session["firstAccess"] = 1;
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (!Session["rank"].Equals("Admin"))
                return RedirectToAction("permission", "Home");

            userDetail userDetail = db.userDetails.Find(id);
            db.userDetails.Remove(userDetail);
            db.SaveChanges();
            Session["success"] = 1;
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
