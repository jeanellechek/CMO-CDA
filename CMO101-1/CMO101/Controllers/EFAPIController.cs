using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CMO101.Models;

namespace CMO101.Controllers
{
    public class EFAPIController : ApiController
    {
        private cmoAzure db = new cmoAzure();

        // GET: api/EFAPI
        public IQueryable<efDetail> GetefDetails()
        {
            var stuff = db.efDetails.OrderBy(c => c.caseID).OrderBy(c => c.dateTime);
            return stuff;
        }

        // GET: api/EFAPI/5
        [ResponseType(typeof(efDTO))]
        public async Task<IHttpActionResult> GetefDetail(int id)
        {
            var store1 = await db.situationDetails.Where(s => s.caseID == id).OrderByDescending(c => c.dateTime).FirstOrDefaultAsync();
            var store2 = await db.caseDetails.Where(c => c.caseID == id).FirstOrDefaultAsync();
            if (store1 == null || store2 == null)
            {
                return NotFound();
            }
            efDTO sendObject = new efDTO()
            {
                caseID = store1.caseID,
                crisisLV = store1.crisisLevel.crisisLevel1,
                actions = store1.actionToDo,
                DT = store1.dateTime,
                unitsDeployed = store1.unitsDeployed,
                informantName = store2.informantName,
                informantPhone = store2.informantPhone,
                location = store2.location,
                lat = store1.lat,
                lng = store1.lng
            };
            

            return Ok(sendObject);
        }

        // PUT: api/EFAPI/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutefDetail(int id, efDetail efDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != efDetail.caseID)
            {
                return BadRequest();
            }

            db.Entry(efDetail).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!efDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/EFAPI
        //[ResponseType(typeof(efDRO))]
        //public async Task<IHttpActionResult> PostefDetail(efDRO efdro)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    string[] text = efdro.action.Split(':');
        //    situationDetail data = db.situationDetails.Where(c => c.caseID == efdro.caseID).OrderByDescending(c => c.dateTime).FirstOrDefault();
        //    situationDetail sitStore = new situationDetail
        //    {
        //        caseID = efdro.caseID,
        //        dateTime = efdro.DT,
        //        casualties = efdro.casualties,
        //        damagedProperties = efdro.damagedProperties,
        //        unitsDeployed = text[0],
        //        remarks = efdro.action,
        //        actionToDo = data.actionToDo,
        //        crisisLevel = data.crisisLevel
        //    };

        //    //add situationDetail
        //    db.situationDetails.Add(sitStore);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        throw;
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = efdro.caseID }, efdro);
        //}

        // DELETE: api/EFAPI/5
        [ResponseType(typeof(efDetail))]
        public async Task<IHttpActionResult> DeleteefDetail(int id)
        {
            efDetail efDetail = await db.efDetails.FindAsync(id);
            if (efDetail == null)
            {
                return NotFound();
            }

            db.efDetails.Remove(efDetail);
            await db.SaveChangesAsync();

            return Ok(efDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool efDetailExists(int id)
        {
            return db.efDetails.Count(e => e.caseID == id) > 0;
        }

        private async void GetfromEF(int id)
        {
            //int id = Int32.Parse(Request["CaseID"].ToString());
            using (var client = new HttpClient())
            {
                string number = id.ToString();
                var uri = new Uri(/*insert EF link here*/"cdaef.azurewebsites.net/api/cdaefapi");
                var response = await client.GetAsync(string.Format("{0}/{1}",uri,number));
                efDRO efdro = await response.Content.ReadAsAsync<efDRO>();
                string[] text = efdro.actions_taken.Split(':');
                situationDetail data = db.situationDetails.Where(c => c.caseID == efdro.incident_id).OrderByDescending(c => c.dateTime).FirstOrDefault();
                situationDetail sitStore = new situationDetail
                {
                    caseID = efdro.incident_id,
                    dateTime = efdro.report_timestamp,
                    casualties = efdro.casualty,
                    damagedProperties = efdro.damage_property,
                    unitsDeployed = text[0],
                    remarks = efdro.actions_taken,
                    actionToDo = data.actionToDo,
                    crisisLevel = data.crisisLevel
                };
                db.situationDetails.Add(sitStore);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    throw;
                }

            }

        }
    }
}