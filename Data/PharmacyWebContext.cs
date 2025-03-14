using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmacyWeb.Models;

namespace PharmacyWeb.Data;

public class PharmacyWebContext : IdentityDbContext
{
	public PharmacyWebContext(DbContextOptions<PharmacyWebContext> options)
		: base(options)
	{
	}

	public DbSet<Medicine> Medicines { get; set; }
	public DbSet<Sale> Sales { get; set; }
	public DbSet<CartItem> CartItems { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Medicine>()
			.Property(m => m.Price)
			.HasPrecision(10, 2);

		modelBuilder.Entity<Sale>()
			.Property(s => s.TotalPrice)
			.HasPrecision(10, 2);
	}
}