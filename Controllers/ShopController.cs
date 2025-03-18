using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyWeb.Interfaces;
using PharmacyWeb.Models;
using System.Security.Claims;

namespace PharmacyWeb.Controllers
{
	public class ShopController : Controller
	{
		private readonly IMedicineRepository _medicineRepository;
		private readonly ICartRepository _cartRepository;
		private readonly ISaleRepository _saleRepository;

		public ShopController(IMedicineRepository medicineRepository, ICartRepository cartRepository, ISaleRepository saleRepository)
		{
			_medicineRepository = medicineRepository;
			_cartRepository = cartRepository;
			_saleRepository = saleRepository;
		}

		// GET: Shop/Index
		public async Task<IActionResult> Index(string category = null)
		{
			IEnumerable<Medicine> medicines;
			if (!string.IsNullOrEmpty(category))
			{
				medicines = await _medicineRepository.GetMedicinesByCategoryAsync(category);
			}
			else
			{
				medicines = await _medicineRepository.GetAllMedicinesAsync();
			}

			var categories = (await _medicineRepository.GetAllMedicinesAsync())
				.Select(m => m.Category)
				.Distinct()
				.Where(c => !string.IsNullOrEmpty(c))
				.OrderBy(c => c)
				.ToList();
			ViewBag.Categories = categories;

			return View(medicines);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> AddToCart(int medicineId, int quantity)
		{
			var medicine = await _medicineRepository.GetMedicineByIdAsync(medicineId);
			if (medicine == null)
			{
				return Json(new { success = false, message = "Thuốc không tồn tại!" });
			}
			if (medicine.StockQuantity < quantity)
			{
				return Json(new { success = false, message = "Không đủ hàng tồn kho!" });
			}

			var cartItem = new CartItem
			{
				UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
				MedicineId = medicineId,
				Quantity = quantity
			};
			await _cartRepository.AddToCartAsync(cartItem);
			return Json(new { success = true, message = "Đã thêm vào giỏ hàng!" });
		}

		[Authorize]
		public async Task<IActionResult> Cart()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var cartItems = await _cartRepository.GetCartItemsAsync(userId);
			return View(cartItems);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> RemoveFromCart(int id)
		{
			await _cartRepository.RemoveFromCartAsync(id);
			return RedirectToAction("Cart");
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Checkout()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var cartItems = await _cartRepository.GetCartItemsAsync(userId);

			if (!cartItems.Any())
			{
				TempData["Error"] = "Giỏ hàng của bạn đang trống!";
				return RedirectToAction("Cart");
			}

			foreach (var item in cartItems)
			{
				// Sử dụng trực tiếp Medicine từ cartItems thay vì tải lại
				var medicine = item.Medicine; // Lấy từ Include trong GetCartItemsAsync
				if (medicine == null)
				{
					TempData["Error"] = "Không tìm thấy thuốc trong giỏ hàng!";
					return RedirectToAction("Cart");
				}
				if (medicine.StockQuantity < item.Quantity)
				{
					TempData["Error"] = $"Không đủ hàng tồn kho cho {medicine.Name}!";
					return RedirectToAction("Cart");
				}

				// Cập nhật StockQuantity
				medicine.StockQuantity -= item.Quantity;
				await _medicineRepository.UpdateMedicineAsync(medicine);

				var sale = new Sale
				{
					MedicineId = item.MedicineId,
					Quantity = item.Quantity,
					TotalPrice = medicine.Price * item.Quantity,
					SaleDate = DateTime.Now,
					CustomerName = User.Identity.Name
				};
				await _saleRepository.AddSaleAsync(sale);
			}

			await _cartRepository.ClearCartAsync(userId);
			TempData["Success"] = "Đã thanh toán thành công!";
			return RedirectToAction("Index");
		}
	}
}