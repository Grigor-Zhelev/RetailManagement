using Microsoft.EntityFrameworkCore;
using StoreApp.Models;

namespace StoreApp.Data
{
	public class StoreDbContext : DbContext
	{
		public StoreDbContext(DbContextOptions<StoreDbContext> options)
			: base(options) { }

		public DbSet<Product> Products { get; set; }
	}
}