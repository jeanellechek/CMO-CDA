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
    public class CMO911APIController : ApiController
    {
        private cmoAzure db = new cmoAzure();

        // GET: api/CMO911API
        public IQueryable<caseDetail> GetcaseDetails()
        {
            return db.caseDetails;
        }

        // GET: api/CMO911API/5
        [ResponseType(typeof(caseDetail))]
        public async Task<IHttpActionResult> GetcaseDetail(int id)
        {
            caseDetail caseDetail = await db.caseDetails.FindAsync(id);
            if (caseDetail == null)
            {
                return NotFound();
            }

            return Ok(caseDetail);
        }

        // PUT: api/CMO911API/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutcaseDetail(int id, caseDetail caseDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != caseDetail.caseID)
            {
                return BadRequest();
            }

            db.Entry(caseDetail).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!caseDetailExists(id))
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

        // POST: api/CMO911API
        [ResponseType(typeof(caseDetail))]
        public async Task<IHttpActionResult> PostcaseDetail(_911DRO _911dro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            caseDetail caseStore = new caseDetail
            {
                caseID = _911dro.caseID,
                crisisLevel = _911dro.crisisLevel,
                dateTime = _911dro.timeStamp,
                description = _911dro.description,
                informantName = _911dro.informantName,
                informantPhone = _911dro.informantNumber,
                location = _911dro.location
            };
            db.caseDetails.Add(caseStore);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (caseDetailExists(_911dro.caseID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = caseStore.caseID }, caseStore);
        }

        // DELETE: api/CMO911API/5
        [ResponseType(typeof(caseDetail))]
        public async Task<IHttpActionResult> DeletecaseDetail(int id)
        {
            caseDetail caseDetail = await db.caseDetails.FindAsync(id);
            if (caseDetail == null)
            {
                return NotFound();
            }

            db.caseDetails.Remove(caseDetail);
            await db.SaveChangesAsync();

            return Ok(caseDetail);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool caseDetailExists(int id)
        {
            return db.caseDetails.Count(e => e.caseID == id) > 0;
        }
    }
}