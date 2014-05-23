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
    public class UrlLinksController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/UrlLinks
        public IQueryable<UrlLink> GetUrlLinks()
        {
            return db.UrlLinks;
        }

        // GET: api/UrlLinks/5
        [ResponseType(typeof(UrlLink))]
        public async Task<IHttpActionResult> GetUrlLink(int id)
        {
            UrlLink urlLink = await db.UrlLinks.FindAsync(id);
            if (urlLink == null)
            {
                return NotFound();
            }

            return Ok(urlLink);
        }

        // PUT: api/UrlLinks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUrlLink(int id, UrlLink urlLink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != urlLink.ID)
            {
                return BadRequest();
            }

            db.Entry(urlLink).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UrlLinkExists(id))
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

        // POST: api/UrlLinks
        [ResponseType(typeof(UrlLink))]
        public async Task<IHttpActionResult> PostUrlLink(UrlLink urlLink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UrlLinks.Add(urlLink);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = urlLink.ID }, urlLink);
        }

        // DELETE: api/UrlLinks/5
        [ResponseType(typeof(UrlLink))]
        public async Task<IHttpActionResult> DeleteUrlLink(int id)
        {
            UrlLink urlLink = await db.UrlLinks.FindAsync(id);
            if (urlLink == null)
            {
                return NotFound();
            }

            db.UrlLinks.Remove(urlLink);
            await db.SaveChangesAsync();

            return Ok(urlLink);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UrlLinkExists(int id)
        {
            return db.UrlLinks.Count(e => e.ID == id) > 0;
        }
    }
}