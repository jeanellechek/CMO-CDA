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
using System.IO;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Infrastructure;

namespace CMO101.Controllers
{
    public class caseDetailsController : Controller
    {
        private cmoAzure db = new cmoAzure();

        // GET: caseDetails
        public async Task<ActionResult> Index(int? newCaseReq)
        {
            if (newCaseReq == 1)
            {
                //stub here
                Getfrom911();
                return RedirectToAction("Index");
            }

            var caseDetails = db.caseDetails.Include(c => c.crisisLevel1).OrderByDescending(c => c.caseID);
            return View(await caseDetails.ToListAsync());
        }

        // GET: caseDetails/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            caseDetail caseDetail = await db.caseDetails.FindAsync(id);
            if (caseDetail == null)
            {
                return HttpNotFound();
            }
            return View(caseDetail);
        }

        // GET: caseDetails/Create
        public ActionResult Create()
        {
            Session["selfCreate"] = "1";
            ViewBag.crisisLevel = new SelectList(db.crisisLevels, "crisisID", "crisisLevel1");
            return View();
        }

        // POST: caseDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "caseID,description,informantName,informantPhone,dateTime,location,crisisLevel,initialLat,initialLng,caseStatus")] caseDetail caseDetail)
        {
            if (ModelState.IsValid)
            {
                db.caseDetails.Add(caseDetail);
                await db.SaveChangesAsync();
               
                return RedirectToAction("Index");
            }

            ViewBag.crisisLevel = new SelectList(db.crisisLevels, "crisisID", "crisisLevel1", caseDetail.crisisLevel);
            return View(caseDetail);
        }

        // GET: caseDetails/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            caseDetail caseDetail = await db.caseDetails.FindAsync(id);
            if (caseDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.crisisLevel = new SelectList(db.crisisLevels, "crisisID", "crisisLevel1", caseDetail.crisisLevel);
            return View(caseDetail);
        }

        // POST: caseDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "caseID,description,informantName,informantPhone,dateTime,location,crisisLevel,initialLat,initialLng,caseStatus")] caseDetail caseDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(caseDetail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.crisisLevel = new SelectList(db.crisisLevels, "crisisID", "crisisLevel1", caseDetail.crisisLevel);
            return View(caseDetail);
        }

        // GET: caseDetails/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            caseDetail caseDetail = await db.caseDetails.FindAsync(id);
            if (caseDetail == null)
            {
                return HttpNotFound();
            }
            return View(caseDetail);
        }

        // POST: caseDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            caseDetail caseDetail = await db.caseDetails.FindAsync(id);
            db.caseDetails.Remove(caseDetail);
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
        private void Getfrom911()
        {
            using (var client = new WebClient())
            {
                var uri = new Uri(/*insert 911 link here*/"https://cda911.azurewebsites.net/api/_911API/getCMO");
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                request.Method = "GET";
                String store = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    store = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }
                var cmo911dro = JObject.Parse(store);
                string x = cmo911dro["caseID"].ToString();
                if (!caseDetailExists(Convert.ToInt32(cmo911dro["caseID"])))
                {
                    caseDetail caseStore = new caseDetail
                    {
                        caseID = Convert.ToInt32(cmo911dro["caseID"]),
                        crisisLevel = Convert.ToInt32(cmo911dro["crisisLevel"]),
                        dateTime = Convert.ToDateTime(cmo911dro["timeStamp"]),
                        description = cmo911dro["description"].ToString(),
                        informantName = cmo911dro["informantName"].ToString(),
                        informantPhone = Convert.ToInt32(cmo911dro["informantNumber"]),
                        location = cmo911dro["location"].ToString(),
                        initialLat = Convert.ToDouble(cmo911dro["lat"]),
                        initialLng = Convert.ToDouble(cmo911dro["lng"]),
                        caseStatus = "Open"
                    };
                    db.caseDetails.Add(caseStore);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateException)
                    {
                        throw;
                    }
                }
            }
        }
        private bool caseDetailExists(int id)
        {
            return db.caseDetails.Count(e => e.caseID == id) > 0;
        }
    }
}
