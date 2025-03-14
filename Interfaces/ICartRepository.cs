using PharmacyWeb.Models;

namespace PharmacyWeb.Interfaces
{
	public interface ICartRepository
	{
		Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId);
		Task AddToCartAsync(CartItem cartItem);
		Task RemoveFromCartAsync(int id);
		Task ClearCartAsync(string userId);
	}
}
