using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyWeb.Models;

public class Sale
{
	public int Id { get; set; }
	public int MedicineId { get; set; }
	public virtual Medicine? Medicine { get; set; }
	public int Quantity { get; set; }
	[Column(TypeName = "decimal(10,2)")]
	public decimal TotalPrice { get; set; }
	public DateTime SaleDate { get; set; }
	public string? CustomerName { get; set; }
}