using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApp.Data;
using StoreApp.Models;
using StoreApp.Services;

namespace StoreApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		private readonly StoreDbContext _context;
		private readonly SyncService _syncService;
		private readonly IConfiguration _configuration;

		public ProductsController(StoreDbContext context, SyncService syncService, IConfiguration configuration)
		{
			_context = context;
			_syncService = syncService;
			_configuration = configuration;
		}

		private Guid StoreId => Guid.Parse(_configuration["StoreInfo:StoreId"]!);
		private string StoreName => _configuration["StoreInfo:StoreName"]!;

		// GET: api/products
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var products = await _context.Products.ToListAsync();
			return Ok(products);
		}

		// GET: api/products/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null) return NotFound();
			return Ok(product);
		}

		// POST: api/products
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Product product)
		{
			product.Id = Guid.NewGuid();
			product.CreatedOn = DateTime.UtcNow;
			product.UpdatedOn = DateTime.UtcNow;

			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			// Sync to Central
			await _syncService.SyncProductAsync(product, StoreId, StoreName);

			return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
		}

		// PUT: api/products/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] Product product)
		{
			var existing = await _context.Products.FindAsync(id);
			if (existing == null) return NotFound();

			existing.Name = product.Name;
			existing.Description = product.Description;
			existing.Price = product.Price;
			existing.MinPrice = product.MinPrice;
			existing.UpdatedOn = DateTime.UtcNow;

			await _context.SaveChangesAsync();

			// Sync to Central
			await _syncService.SyncProductAsync(existing, StoreId, StoreName);

			return Ok(existing);
		}

		// DELETE: api/products/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null) return NotFound();

			_context.Products.Remove(product);
			await _context.SaveChangesAsync();
			return NoContent();
		}

		// POST: api/products/sync — от Central към Store
		[HttpPost("sync")]
		public async Task<IActionResult> Sync([FromBody] Product product)
		{
			var existing = await _context.Products.FindAsync(product.Id);
			if (existing == null)
			{
				product.CreatedOn = DateTime.UtcNow;
				product.UpdatedOn = DateTime.UtcNow;
				_context.Products.Add(product);
			}
			else
			{
				existing.Name = product.Name;
				existing.Description = product.Description;
				existing.Price = product.Price;
				existing.MinPrice = product.MinPrice;
				existing.UpdatedOn = DateTime.UtcNow;
			}

			await _context.SaveChangesAsync();
			return Ok(product);
		}
	}
}