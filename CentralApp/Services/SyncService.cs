using System.Text;
using System.Text.Json;
using CentralApp.Models;

namespace CentralApp.Services
{
	public class SyncService
	{
		private readonly HttpClient _httpClient;

		public SyncService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task SyncToStoreAsync(Product product, string storeUrl)
		{
			var payload = new
			{
				product.Id,
				product.Name,
				product.Description,
				product.Price,
				product.MinPrice,
				product.CreatedOn,
				product.UpdatedOn
			};

			var json = JsonSerializer.Serialize(payload);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			try
			{
				await _httpClient.PostAsync($"{storeUrl}/api/products/sync", content);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Sync to store failed: {ex.Message}");
			}
		}
	}
}