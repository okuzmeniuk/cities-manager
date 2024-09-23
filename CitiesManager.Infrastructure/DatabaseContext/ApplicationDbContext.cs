using CitiesManager.Core.Identity;
using CitiesManager.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Infrastructure.DatabaseContext
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
	{
		public virtual DbSet<City> Cities { get; set; }

		public ApplicationDbContext() { }

		public ApplicationDbContext(DbContextOptions options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<City>().HasData(
				new City(Guid.Parse("55DD4A40-519D-424A-A74A-FE4FAB309249"), "Kyiv"),
				new City(Guid.Parse("4A101B28-C13C-480D-93A9-17159A35D050"), "Lviv")
			);
		}
	}
}
