using WebApplication.Models;

namespace WebApplication.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApplication.Models.TagBoaDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "WebApplication.Models.TagBoaDbContext";
        }

        protected override void Seed(WebApplication.Models.TagBoaDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
			//context.Items.AddOrUpdate(
			//  p => p.FullName,
			//  new Tag { FullName = "Andrew Peters" },
			//  new Person { FullName = "Rowan Miller" }
			//);
            
        }
    }
}
