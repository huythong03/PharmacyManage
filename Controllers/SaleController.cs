using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using PharmacyWeb.Interfaces;
using PharmacyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace PharmacyWeb.Controllers;

[Authorize(Roles = "Admin,Pharmacist")]
public class SaleController : Controller
{
	private readonly IMedicineRepository _medicineRepository;
	private readonly ISaleRepository _saleRepository;

	public SaleController(IMedicineRepository medicineRepository, ISaleRepository saleRepository)
	{
		_medicineRepository = medicineRepository;
		_saleRepository = saleRepository;
	}

	public async Task<IActionResult> Index(string searchString)
	{
		var sales = await _saleRepository.GetAllSalesAsync();

		if (!string.IsNullOrEmpty(searchString))
		{
			sales = sales.Where(s => s.CustomerName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
		}

		return View(sales);
	}

	[HttpGet]
	public async Task<IActionResult> Create()
	{
		var medicines = await _medicineRepository.GetAllMedicinesAsync();
		ViewBag.Medicines = new SelectList(medicines, "Id", "Name");
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(Sale sale)
	{
		if (ModelState.IsValid)
		{
			var medicine = await _medicineRepository.GetMedicineByIdAsync(sale.MedicineId);
			if (medicine == null || !await _medicineRepository.CheckStockAsync(sale.MedicineId, sale.Quantity))
			{
				ModelState.AddModelError("", "Không đủ hàng tồn kho hoặc thuốc không hợp lệ.");
				var medicines = await _medicineRepository.GetAllMedicinesAsync();
				ViewBag.Medicines = new SelectList(medicines, "Id", "Name");
				return View(sale);
			}

			sale.TotalPrice = medicine.Price * sale.Quantity;
			sale.SaleDate = DateTime.Now;

			medicine.StockQuantity -= sale.Quantity;
			await _medicineRepository.UpdateMedicineAsync(medicine);
			await _saleRepository.AddSaleAsync(sale);

			return RedirectToAction(nameof(Index));
		}

		var allMedicines = await _medicineRepository.GetAllMedicinesAsync();
		ViewBag.Medicines = new SelectList(allMedicines, "Id", "Name");
		return View(sale);
	}

	public async Task<IActionResult> DailyReport(DateTime? startDate, DateTime? endDate, string period = "day")
	{
		var start = startDate ?? DateTime.Today.AddDays(-6); // Mặc định 7 ngày trước
		var end = endDate ?? DateTime.Today;
		var revenueData = await _saleRepository.GetRevenueByRangeAsync(start, end, period);
		var sales = await _saleRepository.GetSalesByDateAsync(DateTime.Today); // Giữ nguyên cho ngày hiện tại

		ViewBag.RevenueData = revenueData;
		ViewBag.StartDate = start;
		ViewBag.EndDate = end;
		ViewBag.Period = period;
		return View(sales);
	}
}