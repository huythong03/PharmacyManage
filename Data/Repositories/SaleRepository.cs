using Microsoft.EntityFrameworkCore;
using PharmacyWeb.Interfaces;
using PharmacyWeb.Models;
using System.Globalization;

namespace PharmacyWeb.Data.Repositories;

public class SaleRepository : ISaleRepository
{
	private readonly PharmacyWebContext _context;

	public SaleRepository(PharmacyWebContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Sale>> GetAllSalesAsync()
	{
		return await _context.Sales
			.Include(s => s.Medicine)
			.ToListAsync();
	}

	public async Task<Sale?> GetSaleByIdAsync(int id)
	{
		return await _context.Sales
			.Include(s => s.Medicine)
			.FirstOrDefaultAsync(s => s.Id == id);
	}

	public async Task AddSaleAsync(Sale sale)
	{
		_context.Sales.Add(sale);
		await _context.SaveChangesAsync();
	}

	public async Task<decimal> GetDailyRevenueAsync(DateTime date)
	{
		return await _context.Sales
			.Where(s => s.SaleDate.Date == date.Date)
			.SumAsync(s => s.TotalPrice);
	}

	public async Task<IEnumerable<Sale>> GetSalesByDateAsync(DateTime date)
	{
		return await _context.Sales
			.Include(s => s.Medicine)
			.Where(s => s.SaleDate.Date == date.Date)
			.ToListAsync();
	}

	public async Task<Dictionary<string, decimal>> GetRevenueByRangeAsync(DateTime startDate, DateTime endDate, string period)
	{
		var sales = await _context.Sales
			.Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
			.ToListAsync();

		if (period == "day")
		{
			return sales
				.GroupBy(s => s.SaleDate.ToString("dd/MM/yyyy"))
				.ToDictionary(g => g.Key, g => g.Sum(s => s.TotalPrice));
		}
		else if (period == "week")
		{
			return sales
				.GroupBy(s => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(s.SaleDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString() + "/" + s.SaleDate.Year)
				.ToDictionary(g => g.Key, g => g.Sum(s => s.TotalPrice));
		}
		else // month
		{
			return sales
				.GroupBy(s => s.SaleDate.ToString("MM/yyyy"))
				.ToDictionary(g => g.Key, g => g.Sum(s => s.TotalPrice));
		}
	}
}