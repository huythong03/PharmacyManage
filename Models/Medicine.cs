using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PharmacyWeb.Models;

public class Medicine
{
	public int Id { get; set; }
	[Required]
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	[Required]
	[Column(TypeName = "decimal(10,2)")]
	public decimal Price { get; set; }
	public int StockQuantity { get; set; }
	public DateTime ExpiryDate { get; set; }
	public string? Category { get; set; }
	public string? ImageUrl { get; set; }
}