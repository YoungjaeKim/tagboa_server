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
using WebApplication.Models;

namespace WebApplication.Controllers.api
{
    public class CurricularsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Curriculars
        public IQueryable<Curricular> GetCurriculars()
        {
            return db.Curriculars;
        }

        // GET: api/Curriculars/5
        [ResponseType(typeof(Curricular))]
        public async Task<IHttpActionResult> GetCurricular(int id)
        {
            Curricular curricular = await db.Curriculars.FindAsync(id);
            if (curricular == null)
            {
                return NotFound();
            }

            return Ok(curricular);
        }

        // PUT: api/Curriculars/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCurricular(int id, Curricular curricular)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != curricular.ID)
            {
                return BadRequest();
            }

            db.Entry(curricular).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurricularExists(id))
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

        // POST: api/Curriculars
        [ResponseType(typeof(Curricular))]
        public async Task<IHttpActionResult> PostCurricular(Curricular curricular)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Curriculars.Add(curricular);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = curricular.ID }, curricular);
        }

        // DELETE: api/Curriculars/5
        [ResponseType(typeof(Curricular))]
        public async Task<IHttpActionResult> DeleteCurricular(int id)
        {
            Curricular curricular = await db.Curriculars.FindAsync(id);
            if (curricular == null)
            {
                return NotFound();
            }

            db.Curriculars.Remove(curricular);
            await db.SaveChangesAsync();

            return Ok(curricular);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CurricularExists(int id)
        {
            return db.Curriculars.Count(e => e.ID == id) > 0;
        }
    }
}