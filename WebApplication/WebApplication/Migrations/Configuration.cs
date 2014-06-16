using WebApplication.Models;

namespace WebApplication.Migrations
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<WebApplication.Models.ApplicationDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
			ContextKey = "WebApplication.Models.ApplicationDbContext";
		}

		protected override void Seed(WebApplication.Models.ApplicationDbContext context)
		{
			this.AddUserAndRoles();

			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//
		}

		private bool AddUserAndRoles()
		{
			bool success = false;
			var idManager = new IdentityManager();
			success = idManager.CreateRole("Admin");
			if (!success) return false;
			success = idManager.CreateRole("CanEdit");
			if (!success) return false;
			success = idManager.CreateRole("User");
			if (!success) return false;
			return true;
		}
	}
}
