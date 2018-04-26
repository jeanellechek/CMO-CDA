using CMO101.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMO101.Controllers
{
    public class HomeController : Controller
    {
        private cmoAzure db = new cmoAzure();

        [HttpPost]
        public ActionResult CheckUsername(string EmailAddress, string Password)
        {
            cmoAzure entities = new cmoAzure();
            bool valid = entities.userDetails.ToList().Exists(p => (p.email.Equals(EmailAddress, StringComparison.CurrentCultureIgnoreCase) && p.password.Equals(Password)));
            
            if (valid == true)
            {
                var name = (from h in entities.userDetails where h.email.Equals(EmailAddress) select h.name).FirstOrDefault();
                Session["name"] = name;

                var rank = (from h in entities.userDetails where h.email.Equals(EmailAddress) select h.rank).FirstOrDefault();
                Session["rank"] = rank;

                return RedirectToAction("Home");

            }
            else
                return RedirectToAction("Error");


        }

        public ActionResult Index()
        {
            Session["firstAccess"] = "None";
            Session["rank"] = "None";
            Session["success"] = "None";
            Session["selfCreate"] = "None";
            ViewBag.Title = "Login Page";

            return View();
        }
        public ActionResult Home()
        {
            if(Session["rank"].Equals("None"))
                return RedirectToAction("Index");

            Response.AppendHeader("Refresh", "30");
            dynamic model = new ExpandoObject();
            model.caseDetails = getCaseDetail();
            model.pmoEFDetails = getPMOEFDetail();
            return View(model);
        }
        public IQueryable<caseDetail> getCaseDetail()
        {
            var _db = new cmoAzure();
            IQueryable<caseDetail> query = _db.caseDetails;
            query = from t in query.OrderByDescending(t => t.dateTime).Take(5) select t;
            return query;
        }
        public IQueryable<situationDetail> getPMOEFDetail()
        {
            var _db = new cmoAzure();
            IQueryable<situationDetail> query = _db.situationDetails;
            query = from t in query.OrderByDescending(t => t.dateTime).Take(5) select t;
            return query;
        }

        public ActionResult logout()
        {
            this.Session.Abandon();
            return RedirectToAction("index");
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult permission()
        {
            return View();
        }
        public ActionResult bigscreen(String keyword)
        {
         //   Response.AppendHeader("Refresh", "60");
            dynamic model = new ExpandoObject();
            model.caseDwl = getCaseID();
            model.latLngDetails = getLatLng(keyword);
            return View(model);
        }

        public IQueryable<caseDetail> getCaseID()
        {
            IQueryable<caseDetail> query = db.caseDetails;
            query = from t in query.Where(x => x.caseStatus.Equals("Open")) select t;
            return query;
        }

        public IQueryable<caseDetail> getLatLng(String keyword)
        {
            IQueryable<caseDetail> query = db.caseDetails;

            if (keyword != null)
            {
                int key = Int32.Parse(keyword);
                query = from t in query.Where(x => x.caseID.Equals(key)) select t;
            }
            else   query = from t in query.Where(x => x.caseStatus.Equals("Open")) select t;

            return query;
        }

    }
}

