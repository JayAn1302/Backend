using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarangayEquipmentAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBorrowerEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Borrowers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Borrowers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
