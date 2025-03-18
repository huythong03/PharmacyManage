using Microsoft.EntityFrameworkCore;
using PharmacyWeb.Interfaces;
using PharmacyWeb.Models;

namespace PharmacyWeb.Data.Repositories
{
	public class MedicineRepository : IMedicineRepository
	{
		private readonly PharmacyWebContext _context;

		public MedicineRepository(PharmacyWebContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IEnumerable<Medicine>> GetAllMedicinesAsync()
		{
			return await _context.Medicines.AsNoTracking().ToListAsync();
		}

		public async Task<Medicine?> GetMedicineByIdAsync(int id)
		{
			return await _context.Medicines.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
		}

		public async Task AddMedicineAsync(Medicine medicine)
		{
			if (medicine == null) throw new ArgumentNullException(nameof(medicine));
			_context.Medicines.Add(medicine);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateMedicineAsync(Medicine medicine)
		{
			if (medicine == null) throw new ArgumentNullException(nameof(medicine));
			_context.Entry(medicine).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task DeleteMedicineAsync(int id)
		{
			var medicine = await _context.Medicines.FindAsync(id);
			if (medicine != null)
			{
				_context.Medicines.Remove(medicine);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> CheckStockAsync(int id, int quantity)
		{
			var medicine = await _context.Medicines.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
			return medicine != null && medicine.StockQuantity >= quantity;
		}

		public async Task<IEnumerable<Medicine>> GetLowStockMedicinesAsync(int threshold)
		{
			return await _context.Medicines
				.AsNoTracking()
				.Where(m => m.StockQuantity <= threshold)
				.ToListAsync();
		}

		public async Task<IEnumerable<Medicine>> GetExpiringMedicinesAsync(int days)
		{
			var thresholdDate = DateTime.Now.AddDays(days);
			return await _context.Medicines
				.AsNoTracking()
				.Where(m => m.ExpiryDate <= thresholdDate && m.ExpiryDate >= DateTime.Now)
				.ToListAsync();
		}

		public async Task<IEnumerable<Medicine>> GetMedicinesByCategoryAsync(string category)
		{
			return await _context.Medicines
				.AsNoTracking()
				.Where(m => m.Category == category)
				.ToListAsync();
		}
	}
}