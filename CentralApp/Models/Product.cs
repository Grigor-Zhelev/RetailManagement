namespace CentralApp.Models
{
	public class Product
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public decimal MinPrice { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime UpdatedOn { get; set; }
		public Guid StoreId { get; set; }
		public string StoreName { get; set; } = string.Empty;
	}
}