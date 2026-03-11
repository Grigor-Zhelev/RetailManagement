using Microsoft.EntityFrameworkCore;
using CentralApp.Models;

namespace CentralApp.Data
{
	public class CentralDbContext : DbContext
	{
		public CentralDbContext(DbContextOptions<CentralDbContext> options)
			: base(options) { }

		public DbSet<Product> Products { get; set; }
	}
}