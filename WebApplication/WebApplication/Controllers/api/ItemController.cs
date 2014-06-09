using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication.Models;

namespace WebApplication.Controllers.api
{
	public class ItemController : ApiController
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET api/Item
		public List<Item> GetItems()
		{
			return db.Items.ToList();
		}

		// GET api/Item/5
		[ResponseType(typeof(Item))]
		public IHttpActionResult GetItem(int id)
		{
			Item item = db.Items.Find(id);
			if (item == null)
			{
				return NotFound();
			}

			return Ok(item);
		}

		// GET api/Item/5
		[Authorize]
		public List<Item> GetItem(string username)
		{
			var item = db.Items.Where(f => f.Author.Equals(username));

			return item.ToList();
		}


		// PUT api/Item/5
		[Authorize]
		public IHttpActionResult PutItem(int id, Item item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != item.ID)
			{
				return BadRequest();
			}

			db.Entry(item).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ItemExists(id))
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

		// POST api/Item
		[Authorize]
		[ResponseType(typeof(Item))]
		public IHttpActionResult PostItem(Item item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (String.IsNullOrEmpty(item.Author))
				item.Author = User.Identity.Name;

			db.Items.Add(item);
			db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = item.ID }, item);
		}

		// DELETE api/Item/5
		[Authorize]
		[ResponseType(typeof(Item))]
		public IHttpActionResult DeleteItem(int id)
		{
			Item item = db.Items.Find(id);
			if (item == null)
			{
				return NotFound();
			}

			db.Items.Remove(item);
			db.SaveChanges();

			return Ok(item);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool ItemExists(int id)
		{
			return db.Items.Count(e => e.ID == id) > 0;
		}
	}
}