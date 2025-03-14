using PharmacyWeb.Models;

namespace PharmacyWeb.Interfaces
{
	public interface IMedicineRepository
	{
		Task<IEnumerable<Medicine>> GetAllMedicinesAsync();
		Task<Medicine?> GetMedicineByIdAsync(int id);
		Task AddMedicineAsync(Medicine medicine);
		Task UpdateMedicineAsync(Medicine medicine);
		Task DeleteMedicineAsync(int id);
		Task<bool> CheckStockAsync(int id, int quantity);
		Task<IEnumerable<Medicine>> GetLowStockMedicinesAsync(int threshold);
		Task<IEnumerable<Medicine>> GetExpiringMedicinesAsync(int days);
		Task<IEnumerable<Medicine>> GetMedicinesByCategoryAsync(string category);
	}
}