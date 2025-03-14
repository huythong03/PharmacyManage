using PharmacyWeb.Models;

namespace PharmacyWeb.Interfaces;

public interface ISaleRepository
{
	Task<IEnumerable<Sale>> GetAllSalesAsync();
	Task<Sale?> GetSaleByIdAsync(int id);
	Task AddSaleAsync(Sale sale);
	Task<decimal> GetDailyRevenueAsync(DateTime date);
	Task<IEnumerable<Sale>> GetSalesByDateAsync(DateTime date);
	Task<Dictionary<string, decimal>> GetRevenueByRangeAsync(DateTime startDate, DateTime endDate, string period);
}