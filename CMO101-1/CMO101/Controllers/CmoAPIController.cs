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
    //where is the pmo api controller? wait
    public class CmoAPIController : ApiController
    {
        private cmoAzure db = new cmoAzure();

        // GET: api/CmoAPI
        public IQueryable<caseDetail> GetcaseDetails()
        {
            return db.caseDetails;
        }

        // GET: api/CmoAPI/5
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

        // PUT: api/CmoAPI/5
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

        // POST: api/CmoAPI
        [ResponseType(typeof(caseDetail))]
        public async Task<IHttpActionResult> PostcaseDetail(caseDetail caseDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.caseDetails.Add(caseDetail);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (caseDetailExists(caseDetail.caseID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = caseDetail.caseID }, caseDetail);
        }

        //attempt to insert SituationDetail
        [ResponseType(typeof(situationDetail))]
        public async Task<IHttpActionResult> PostsituationDetail(situationDetail sit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.situationDetails.Add(sit);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (caseDetailExists(sit.caseID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = sit.caseID }, sit);
        }

        //testing PATCH
        [HttpPatch]
        public async Task<IHttpActionResult> PatchsituationDetail(Int32 cID,string todo,string remark,string units)
        {
            situationDetail sdPatch = db.situationDetails.Find(cID);
            sdPatch.actionToDo = todo;
            sdPatch.remarks = remark;
            sdPatch.unitsDeployed = units;
            try
            {
                await db.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                if(situationDetailExists(cID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }

        // DELETE: api/CmoAPI/5
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
        private bool situationDetailExists(int id)
        {
            return db.situationDetails.Count(e => e.caseID == id) > 0;
        }
    }
}