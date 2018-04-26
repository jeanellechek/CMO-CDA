using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using CMO101.Models;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CMO101.Controllers
{
    public class situationDetailsController : Controller
    {
        private cmoAzure db = new cmoAzure();

        // GET: situationDetails

        public ActionResult Index(int? id, int? getEFDetails,int? getPMODetails, int? getFinalReport)
        {
            Response.AppendHeader("Refresh", "30");
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");

            if (getEFDetails == 1)
            {
                //call EF API here
                GetfromEF(id);
                Response.AppendHeader("Refresh", "1");
            }
            else if (getPMODetails == 1)
            { //call PMO API here
                GetfromPMO(id);
                Response.AppendHeader("Refresh", "1");
            }
            else if (getFinalReport == 1)
            {
                generateFinalReport(id);
                Response.AppendHeader("Refresh", "1");
            }
            if (id == null)
            {
                id = Int32.Parse(Session["editCase"].ToString());
            }
            Session["editCase"] = id;
            dynamic model = new ExpandoObject();
            model.caseDetails = getCaseDetail(Int32.Parse(Session["editCase"].ToString()));
            model.situationDetails = getSituationDetails(Int32.Parse(Session["editCase"].ToString()));
            foreach (situationDetail sd in model.situationDetails)
            {
                sd.dateTime = sd.dateTime.AddHours(8);
            }
          
            return View(model);
        }


        public IQueryable<caseDetail> getCaseDetail(int id)
        {
            var _db = new cmoAzure();
            IQueryable<caseDetail> query = _db.caseDetails;
            query =from t in query.Where(s => s.caseID == id) select t;
            return query;
        }

        public IQueryable<situationDetail> getSituationDetails(int id)
        {
            var _db = new cmoAzure();
            IQueryable<situationDetail> query = _db.situationDetails;
            query = from t in query.Where(s => s.caseID == id).OrderByDescending(s=>s.dateTime) select t;
            int recordCount = query.Count();
            if (recordCount <= 0)
            { //create empty record
                situationDetail ord = new situationDetail();
              
                {
                    ord.caseID = id;
                    ord.crisisID = db.caseDetails.Where(u => u.caseID == id).Select(u => u.crisisLevel).First();
                    ord.actionToDo = "-";
                    ord.dateTime = db.caseDetails.Where(u => u.caseID == id).Select(u => u.dateTime).First();
                    ord.casualties = 0;
                    ord.damagedProperties = "-";
                    ord.unitsDeployed = "-";
                    if (!Session["selfCreate"].Equals("1"))
                        ord.remarks = "New case received from 911"; 
                    else
                    {
                        ord.remarks = "Manual create case";
                        Session["selfCreate"] = "None";
                    }
                       

                    ord.lat = db.caseDetails.Where(u => u.caseID == id).Select(u => u.initialLat).First(); ; //convert address to lat lng in js
                    ord.lng = db.caseDetails.Where(u => u.caseID == id).Select(u => u.initialLng).First(); ; //convert address to lat lng in js


                };

                // Add the new object to the Orders collection.
                db.situationDetails.Add(ord);
                db.SaveChanges();
            }
            return query;
        }

        // GET: situationDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            situationDetail situationDetail = db.situationDetails.Find(id);
            if (situationDetail == null)
            {
                return HttpNotFound();
            }
            return View(situationDetail);
        }

        // GET: situationDetails/Create
        public ActionResult Create()
        {
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            ViewBag.caseID = new SelectList(db.caseDetails, "caseID", "description");
            ViewBag.crisisID = new SelectList(db.crisisLevels, "crisisID", "crisisLevel1");
            return View();
        }


        // POST: situationDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "caseID,crisisID,actionToDo,dateTime,casualties,damagedProperties,unitsDeployed,remarks")] situationDetail situationDetail)
        {
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            Session["firstAccess"] = 1;
            if (ModelState.IsValid)
            {
                db.situationDetails.Add(situationDetail);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = situationDetail.caseID });
            }
            else
            {
                ViewBag.caseID = new SelectList(db.caseDetails, "caseID", "description", situationDetail.caseID);
                ViewBag.crisisID = new SelectList(db.crisisLevels, "crisisID", "crisisLevel1", situationDetail.crisisID);
                return RedirectToAction("Index");
            }
            

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult update([Bind(Include = "caseID,crisisID,actionToDo,dateTime,casualties,damagedProperties,unitsDeployed,remarks")] situationDetail situationDetail)
        {
            Session["firstAccess"] = 1;
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (Session["rank"].Equals("Operator"))
                return RedirectToAction("permission", "Home");
           
            if (ModelState.IsValid)
            {
                db.situationDetails.Add(situationDetail);
                Session["success"]=db.SaveChanges();
                return RedirectToAction("Index", new { id = situationDetail.caseID });
            }
            else
            {
                ViewBag.caseID = new SelectList(db.caseDetails, "caseID", "description", situationDetail.caseID);
                ViewBag.crisisID = new SelectList(db.crisisLevels, "crisisID", "crisisLevel1", situationDetail.crisisID);
                return RedirectToAction("Index");
            }
            

        }

        // GET: situationDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (Session["rank"].Equals("Operator"))
                return RedirectToAction("permission", "Home");
            else if (Session["rank"].Equals("Officer"))
                return RedirectToAction("permission", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            situationDetail situationDetail = db.situationDetails.Find(id);
            if (situationDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.caseID = new SelectList(db.caseDetails, "caseID", "description", situationDetail.caseID);
            ViewBag.crisisID = new SelectList(db.crisisLevels, "crisisID", "crisisLevel1", situationDetail.crisisID);
            return View(situationDetail);
        }

        public ActionResult update(int? id)
        {
            Session["firstAccess"] = 1;
            if (Session["rank"].Equals("None") || (Session["rank"].Equals("Operator")))
                return RedirectToAction("Index", "Home");
            else if (Session["rank"].Equals("Operator"))
                return RedirectToAction("permission", "Home");
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            situationDetail situationDetail = db.situationDetails.OrderByDescending(x => x.recordID).FirstOrDefault(x => x.caseID == id);
            if (situationDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.caseID = new SelectList(db.caseDetails, "caseID", "description", situationDetail.caseID);
            ViewBag.crisisID = new SelectList(db.crisisLevels, "crisisID", "crisisLevel1", situationDetail.crisisID);
           
            return View(situationDetail);
        }

        // POST: situationDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "recordID,caseID,crisisID,actionToDo,dateTime,casualties,damagedProperties,unitsDeployed,remarks")] situationDetail situationDetail)
        {
            Session["firstAccess"] = 1;
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            else if (Session["rank"].Equals("Operator"))
                return RedirectToAction("permission", "Home");
            else if (Session["rank"].Equals("Officer"))
                return RedirectToAction("permission", "Home");
           
            if (ModelState.IsValid)
            {
                db.Entry(situationDetail).State = EntityState.Modified;
                db.SaveChanges();
                Session["success"] = 1;
                Session["edited"] = 1;
                return RedirectToAction("Index");
            }
            ViewBag.caseID = new SelectList(db.caseDetails, "caseID", "description", situationDetail.caseID);
            ViewBag.crisisID = new SelectList(db.crisisLevels, "crisisID", "crisisLevel1", situationDetail.crisisID);
            return View(situationDetail);
        }

        // GET: situationDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            Session["firstAccess"] = 1;
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            
            Session["success"] = 1;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            situationDetail situationDetail = db.situationDetails.Find(id);
            if (situationDetail == null)
            {
                return HttpNotFound();
            }
            return View(situationDetail);
        }

        // POST: situationDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["rank"].Equals("None"))
                return RedirectToAction("Index", "Home");
            situationDetail situationDetail = db.situationDetails.Find(id);
            db.situationDetails.Remove(situationDetail);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = Session["editCase"] });
        }

        private void GetfromEF(int? id)
        {
            //int id = Int32.Parse(Request["CaseID"].ToString());
            using (var client = new WebClient())
            {
                string number = id.ToString();
                var uri = new Uri(/*insert EF link here */ "https://cdaef.azurewebsites.net/api/cdaefapi");
                //var response = client.DownloadString(string.Format("{0}/{1}", uri, number));
                //    //client.GetAsync(string.Format("{0}/{1}", uri, number));
                //efDRO efdro = JsonConv
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}/{1}", uri, number));
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
                var efdro = JObject.Parse(store);
                string[] text = efdro["actions_taken"].ToString().Split(':');
                int caseID = Convert.ToInt32(efdro["incident_id"].ToString());
                situationDetail data = db.situationDetails.Where(c => c.caseID == caseID).OrderByDescending(c => c.dateTime).FirstOrDefault();
                situationDetail sitStore = new situationDetail
                {
                    caseID = caseID,
                    dateTime = Convert.ToDateTime(efdro["report_timestamp"].ToString()),
                    casualties = Convert.ToInt32(efdro["casualty"]),
                    damagedProperties = efdro["damage_property"].ToString(),
                    unitsDeployed = text[0],
                    remarks = efdro["actions_taken"].ToString(),
                    actionToDo = data.actionToDo,
                    crisisLevel = data.crisisLevel
                };
                db.situationDetails.Add(sitStore);

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

        private void GetfromPMO(int? id)
        {
            //int id = Int32.Parse(Request["CaseID"].ToString());
            using (var client = new WebClient())
            {
                string number = id.ToString();
                var uri = new Uri(/*insert EF link here */ "https://pmowebapp.azurewebsites.net/api/forcmo");
                //var response = client.DownloadString(string.Format("{0}/{1}", uri, number));
                //    //client.GetAsync(string.Format("{0}/{1}", uri, number));
                //efDRO efdro = JsonConv
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}/{1}", uri, number));
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
                var pmodro = JObject.Parse(store);
                int caseID = Convert.ToInt32(pmodro["CaseID"].ToString());
                situationDetail sitStore = db.situationDetails.Where(s => s.caseID == caseID).OrderByDescending(s => s.recordID).FirstOrDefault();
                sitStore.remarks = pmodro["Feedback"].ToString();
                sitStore.dateTime = DateTime.Now;
                db.situationDetails.Add(sitStore);
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void generateFinalReport(int? id)
        {
            int caseid = Convert.ToInt32(id.ToString());
            situationDetail store = db.situationDetails.Where(c => c.caseID == caseid).OrderByDescending(c => c.dateTime).FirstOrDefault();
            store.remarks = "Report generated.";
            store.dateTime = DateTime.Now;
            store.caseDetail.caseStatus = "Closed";
            db.situationDetails.Add(store);
            db.SaveChanges();
        }
    }
}
