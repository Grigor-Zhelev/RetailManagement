using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentralApp.Data;
using CentralApp.Models;
using CentralApp.Services;

namespace CentralApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		private readonly CentralDbContext _context;
		private readonly SyncService _syncService;
		private readonly IConfiguration _configuration;

		public ProductsController(CentralDbContext context, SyncService syncService, IConfiguration configuration)
		{
			_context = context;
			_syncService = syncService;
			_configuration = configuration;
		}

		private string? GetStoreUrl(Guid storeId)
		{
			return _configuration[$"Stores:{storeId}"];
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var products = await _context.Products.ToListAsync();
			return Ok(products);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null) return NotFound();
			return Ok(product);
		}

		[HttpGet("store/{storeId}")]
		public async Task<IActionResult> GetByStore(Guid storeId)
		{
			var products = await _context.Products
				.Where(p => p.StoreId == storeId)
				.ToListAsync();
			return Ok(products);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Product product)
		{
			product.Id = Guid.NewGuid();
			product.CreatedOn = DateTime.UtcNow;
			product.UpdatedOn = DateTime.UtcNow;

			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			// Sync to specific store
			var storeUrl = GetStoreUrl(product.StoreId);
			if (storeUrl != null)
				await _syncService.SyncToStoreAsync(product, storeUrl);

			return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
		}

		[HttpPost("sync")]
		public async Task<IActionResult> Sync([FromBody] Product product)
		{
			var existing = await _context.Products.FindAsync(product.Id);
			if (existing == null)
			{
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

			// Sync to specific store
			var storeUrl = GetStoreUrl(existing.StoreId);
			if (storeUrl != null)
				await _syncService.SyncToStoreAsync(existing, storeUrl);

			return Ok(existing);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null) return NotFound();

			_context.Products.Remove(product);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}