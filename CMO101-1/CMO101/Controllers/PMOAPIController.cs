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
    public class PMOAPIController : ApiController
    {
        private cmoAzure db = new cmoAzure();

        // GET: api/PMOAPI
        // 10 latest
        [ResponseType(typeof(pmoDTO))]
        public async Task<IHttpActionResult> GetsituationDetails()
        {
            var store = await db.situationDetails.OrderByDescending(c => c.dateTime).FirstOrDefaultAsync();
            situationDetail[] test3 = await db.situationDetails.OrderByDescending(c => c.dateTime).ToArrayAsync();
            if (store == null)
            {
                return NotFound();
            }
            pmoDTO[] test2 = new pmoDTO[10];
            
            for(int i = 0; i<10; i++)
            {
                test2[i] = new pmoDTO()
                {
                    caseID = test3[i].caseID,
                    crisisLV = test3[i].crisisLevel.crisisLevel1,
                    casualties = test3[i].casualties,
                    damagedProperties = test3[i].damagedProperties,
                    DT = test3[i].dateTime,
                    unitsDeployed = test3[i].unitsDeployed,
                    actionToDo = test3[i].actionToDo,
                };
            }
            pmoDTO test = new pmoDTO()
            {
                caseID = store.caseID,
                crisisLV = store.crisisLevel.crisisLevel1,
                casualties = store.casualties,
                damagedProperties = store.damagedProperties,
                DT = store.dateTime,
                unitsDeployed = store.unitsDeployed,
                actionToDo = store.actionToDo,

            };
            return Ok(test2);
        }

        // GET: api/PMOAPI/5
        // Single latest update in case specified by id
        [ResponseType(typeof(pmoDTO))]
        public async Task<IHttpActionResult> GetsituationDetail(int id)
        {
            var store = await db.situationDetails.Where(c=>c.caseID == id).OrderByDescending(c => c.dateTime).FirstOrDefaultAsync();
            string toSend = String.Empty;
			string totalDamage = String.Empty;
			int totalCasualties = 0;
			int index = 0;
            foreach (situationDetail a in db.situationDetails.Where(c => c.caseID == id).OrderBy(c => c.recordID))
            {
                toSend += a.dateTime + " " + a.remarks + ";";
				if (index != 0)
				{	
					if (a.damagedProperties != "")
						totalDamage += a.damagedProperties + ";";
					totalCasualties += a.casualties;
				}
				index++;
            }
            if (store == null)
            {
                return NotFound();
            }
            pmoDTO test;
            if (store.caseDetail.caseStatus == "Open")
            {
                test = new pmoDTO()
                {
                    caseID = store.caseID,
                    crisisLV = store.crisisLevel.crisisLevel1,
                    casualties = store.casualties,
                    damagedProperties = store.damagedProperties,
                    DT = store.dateTime,
                    unitsDeployed = store.unitsDeployed,
                    actionToDo = store.remarks,
                    lat = store.caseDetail.initialLat,
                    lng = store.caseDetail.initialLng,
                    caseStatus = store.caseDetail.caseStatus
                };
            }
            else
            {
                test = new pmoDTO()
                {
                    caseID = store.caseID,
                    crisisLV = store.crisisLevel.crisisLevel1,
                    casualties = totalCasualties,
                    damagedProperties = totalDamage,
                    DT = store.dateTime,
                    unitsDeployed = store.unitsDeployed,
                    actionToDo = toSend,
                    lat = store.caseDetail.initialLat,
                    lng = store.caseDetail.initialLng,
                    caseStatus = store.caseDetail.caseStatus
                };
            }

            return Ok(test);
        }

        // PUT: api/PMOAPI/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutsituationDetail(int id, situationDetail situationDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != situationDetail.recordID)
            {
                return BadRequest();
            }

            db.Entry(situationDetail).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!situationDetailExists(id))
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

        // POST: api/PMOAPI
        //[ResponseType(typeof(pmoDRO))]
        //public async Task<IHttpActionResult> PostsituationDetail(pmoDRO pmodro)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    situationDetail sitStore = db.situationDetails.Where(s => s.caseID == pmodro.caseID).OrderByDescending(s => s.recordID).FirstOrDefault();
        //    sitStore.remarks = pmodro.feedback;
        //    sitStore.dateTime = DateTime.Now;
        //    db.situationDetails.Add(sitStore);
        //    //Sends deployment details to EF if PMO sends an approval
        //    if (pmodro.approval.ToLower() == "approved")
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            try
        //            {
        //                var efd = from a in db.situationDetails.Where(s => s.caseID == pmodro.caseID).OrderByDescending(c => c.dateTime)
        //                          from b in db.caseDetails.Where(c => c.caseID == pmodro.caseID)
        //                          select new efDTO()
        //                          {
        //                              caseID = a.caseID,
        //                              crisisLV = a.crisisLevel.crisisLevel1,
        //                              actions = a.actionToDo,
        //                              DT = a.dateTime,
        //                              unitsDeployed = a.unitsDeployed,
        //                              informantName = b.informantName,
        //                              informantPhone = b.informantPhone,
        //                              location = b.location,
        //                              lat = a.lat,
        //                              lng = a.lng
        //                          };
        //                client.BaseAddress = new Uri(/*insert ef link here*/"cdaef.azurewebsites.net/api/cdaefapi");
        //                //http POST
        //                var postTask = client.PostAsJsonAsync<efDTO>(client.BaseAddress, efd.FirstOrDefault());
        //                postTask.Wait();
        //            }
        //            catch
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (situationDetailExists(sitStore.recordID))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = sitStore.recordID }, sitStore);
        //}

        // DELETE: api/PMOAPI/5
        [ResponseType(typeof(situationDetail))]
        public async Task<IHttpActionResult> DeletesituationDetail(int id)
        {
            situationDetail situationDetail = await db.situationDetails.FindAsync(id);
            if (situationDetail == null)
            {
                return NotFound();
            }

            db.situationDetails.Remove(situationDetail);
            await db.SaveChangesAsync();

            return Ok(situationDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool situationDetailExists(int id)
        {
            return db.situationDetails.Count(e => e.recordID == id) > 0;
        }
    }
}