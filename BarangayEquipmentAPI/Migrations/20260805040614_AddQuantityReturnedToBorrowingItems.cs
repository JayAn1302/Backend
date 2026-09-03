using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarangayEquipmentAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityReturnedToBorrowingItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityReturned",
                table: "BorrowingItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityReturned",
                table: "BorrowingItems");
        }
    }
}
