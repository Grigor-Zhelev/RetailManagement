using System.Text;
using System.Text.Json;
using StoreApp.Models;

namespace StoreApp.Services
{
	public class SyncService
	{
		private readonly HttpClient _httpClient;
		private readonly string _centralAppUrl;

		public SyncService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_centralAppUrl = configuration["CentralApp:BaseUrl"] ?? "http://localhost:5000";
		}

		public async Task SyncProductAsync(Product product, Guid storeId, string storeName)
		{
			var payload = new
			{
				product.Id,
				product.Name,
				product.Description,
				product.Price,
				product.MinPrice,
				product.CreatedOn,
				product.UpdatedOn,
				StoreId = storeId,
				StoreName = storeName
			};

			var json = JsonSerializer.Serialize(payload);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			try
			{
				await _httpClient.PostAsync($"{_centralAppUrl}/api/products/sync", content);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Sync failed: {ex.Message}");
			}
		}
	}
}