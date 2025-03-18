using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyWeb.Migrations
{
	/// <inheritdoc />
	public partial class AddImageUrlToMedicines : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Kiểm tra và thêm cột ImageUrl chỉ khi nó chưa tồn tại
			migrationBuilder.Sql(@"IF NOT EXISTS (
                SELECT 1 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'Medicines' 
                AND COLUMN_NAME = 'ImageUrl')
                BEGIN
                    ALTER TABLE [Medicines] ADD [ImageUrl] nvarchar(max) NULL
                END");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Xóa cột ImageUrl khi rollback (nếu tồn tại)
			migrationBuilder.Sql(@"IF EXISTS (
                SELECT 1 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'Medicines' 
                AND COLUMN_NAME = 'ImageUrl')
                BEGIN
                    ALTER TABLE [Medicines] DROP COLUMN [ImageUrl]
                END");
		}
	}
}