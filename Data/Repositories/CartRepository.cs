using Microsoft.EntityFrameworkCore;
using PharmacyWeb.Interfaces;
using PharmacyWeb.Models;

namespace PharmacyWeb.Data.Repositories
{
	public class CartRepository : ICartRepository
	{
		private readonly PharmacyWebContext _context;

		public CartRepository(PharmacyWebContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId)
		{
			return await _context.CartItems
				.Include(c => c.Medicine)
				.Where(c => c.UserId == userId)
				.ToListAsync();
		}

		public async Task AddToCartAsync(CartItem cartItem)
		{
			var existingItem = await _context.CartItems
				.FirstOrDefaultAsync(c => c.UserId == cartItem.UserId && c.MedicineId == cartItem.MedicineId);

			if (existingItem != null)
			{
				existingItem.Quantity += cartItem.Quantity;
			}
			else
			{
				_context.CartItems.Add(cartItem);
			}
			await _context.SaveChangesAsync();
		}

		public async Task RemoveFromCartAsync(int id)
		{
			var item = await _context.CartItems.FindAsync(id);
			if (item != null)
			{
				_context.CartItems.Remove(item);
				await _context.SaveChangesAsync();
			}
		}

		public async Task ClearCartAsync(string userId)
		{
			var items = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
			_context.CartItems.RemoveRange(items);
			await _context.SaveChangesAsync();
		}
	}
}
