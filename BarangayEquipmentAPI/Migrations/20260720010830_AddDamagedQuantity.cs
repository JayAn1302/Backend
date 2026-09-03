using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarangayEquipmentAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDamagedQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DamagedQuantity",
                table: "Equipments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DamagedQuantity",
                table: "Equipments");
        }
    }
}
