using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication.Models
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
		}

		public DbSet<Item> Items { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<UrlLink> UrlLinks { get; set; }
		public DbSet<Curricular> Curriculars { get; set; }
	}
}