using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyWeb.Interfaces;
using PharmacyWeb.Models;

namespace PharmacyWeb.Controllers
{
	[Authorize(Roles = "Admin")]
	public class MedicineController : Controller
	{
		private readonly IMedicineRepository _medicineRepository;

		public MedicineController(IMedicineRepository medicineRepository)
		{
			_medicineRepository = medicineRepository;
		}

		// GET: Medicine/Index
		public async Task<IActionResult> Index(string category = null, string searchTerm = null)
		{
			IEnumerable<Medicine> medicines;
			if (!string.IsNullOrEmpty(category))
			{
				medicines = await _medicineRepository.GetMedicinesByCategoryAsync(category);
			}
			else if (!string.IsNullOrEmpty(searchTerm))
			{
				medicines = (await _medicineRepository.GetAllMedicinesAsync())
					.Where(m => m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				medicines = await _medicineRepository.GetAllMedicinesAsync();
			}

			// Lấy danh sách danh mục duy nhất từ cơ sở dữ liệu
			var categories = (await _medicineRepository.GetAllMedicinesAsync())
				.Select(m => m.Category)
				.Distinct()
				.Where(c => !string.IsNullOrEmpty(c))
				.OrderBy(c => c)
				.ToList();
			ViewBag.Categories = categories;

			// Nếu là yêu cầu AJAX, trả về JSON
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				var result = medicines.Select(m => new
				{
					m.Id,
					m.Name,
					m.Price,
					m.StockQuantity,
					ExpiryDate = m.ExpiryDate.ToString("yyyy-MM-dd"),
					m.Category
				});
				return Json(result);
			}

			ViewBag.LowStockMedicines = await _medicineRepository.GetLowStockMedicinesAsync(10);
			ViewBag.ExpiringMedicines = await _medicineRepository.GetExpiringMedicinesAsync(30);
			return View(medicines);
		}

		// GET: Medicine/SearchMedicines?term={searchTerm}
		public async Task<IActionResult> SearchMedicines(string term)
		{
			var medicines = (await _medicineRepository.GetAllMedicinesAsync())
				.Where(m => m.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
				.Select(m => new
				{
					m.Id,
					m.Name,
					m.Price,
					m.StockQuantity,
					ExpiryDate = m.ExpiryDate.ToString("yyyy-MM-dd"),
					m.Category
				})
				.ToList();
			return Json(medicines);
		}

		// GET: Medicine/Create
		public async Task<IActionResult> Create()
		{
			// Lấy danh sách danh mục từ cơ sở dữ liệu
			var categories = (await _medicineRepository.GetAllMedicinesAsync())
				.Select(m => m.Category)
				.Distinct()
				.Where(c => !string.IsNullOrEmpty(c))
				.OrderBy(c => c)
				.ToList();
			ViewBag.Categories = categories.Any() ? categories : new List<string> { "Chưa có danh mục" };
			return View();
		}

		// POST: Medicine/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Name,Description,Price,StockQuantity,ExpiryDate,Category")] Medicine medicine)
		{
			if (ModelState.IsValid)
			{
				await _medicineRepository.AddMedicineAsync(medicine);
				TempData["Success"] = "Thêm thuốc thành công!";
				return RedirectToAction(nameof(Index));
			}
			// Lấy danh sách danh mục từ cơ sở dữ liệu
			var categories = (await _medicineRepository.GetAllMedicinesAsync())
				.Select(m => m.Category)
				.Distinct()
				.Where(c => !string.IsNullOrEmpty(c))
				.OrderBy(c => c)
				.ToList();
			ViewBag.Categories = categories.Any() ? categories : new List<string> { "Chưa có danh mục" };
			return View(medicine);
		}

		// GET: Medicine/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var medicine = await _medicineRepository.GetMedicineByIdAsync(id.Value);
			if (medicine == null)
			{
				return NotFound();
			}
			// Lấy danh sách danh mục từ cơ sở dữ liệu
			var categories = (await _medicineRepository.GetAllMedicinesAsync())
				.Select(m => m.Category)
				.Distinct()
				.Where(c => !string.IsNullOrEmpty(c))
				.OrderBy(c => c)
				.ToList();
			ViewBag.Categories = categories.Any() ? categories : new List<string> { "Chưa có danh mục" };
			return View(medicine);
		}

		// POST: Medicine/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,StockQuantity,ExpiryDate,Category")] Medicine medicine)
		{
			if (id != medicine.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					await _medicineRepository.UpdateMedicineAsync(medicine);
					TempData["Success"] = "Cập nhật thuốc thành công!";
				}
				catch (Exception)
				{
					if (await _medicineRepository.GetMedicineByIdAsync(medicine.Id) == null)
					{
						return NotFound();
					}
					throw;
				}
				return RedirectToAction(nameof(Index));
			}
			// Lấy danh sách danh mục từ cơ sở dữ liệu
			var categories = (await _medicineRepository.GetAllMedicinesAsync())
				.Select(m => m.Category)
				.Distinct()
				.Where(c => !string.IsNullOrEmpty(c))
				.OrderBy(c => c)
				.ToList();
			ViewBag.Categories = categories.Any() ? categories : new List<string> { "Chưa có danh mục" };
			return View(medicine);
		}

		// GET: Medicine/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var medicine = await _medicineRepository.GetMedicineByIdAsync(id.Value);
			if (medicine == null)
			{
				return NotFound();
			}
			return View(medicine);
		}

		// POST: Medicine/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var medicine = await _medicineRepository.GetMedicineByIdAsync(id);
			if (medicine != null)
			{
				await _medicineRepository.DeleteMedicineAsync(id);
				TempData["Success"] = "Xóa thuốc thành công!";
			}
			return RedirectToAction(nameof(Index));
		}
	}
}