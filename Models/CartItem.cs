namespace PharmacyWeb.Models;

public class CartItem
{
	public int Id { get; set; }
	public string UserId { get; set; }
	public int MedicineId { get; set; }
	public virtual Medicine Medicine { get; set; }
	public int Quantity { get; set; }
}