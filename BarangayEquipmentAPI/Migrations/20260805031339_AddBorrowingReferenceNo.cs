using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarangayEquipmentAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowingReferenceNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceNo",
                table: "Borrowings",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
        UPDATE Borrowings
        SET ReferenceNo =
            'BRN-' + RIGHT('0000' + CAST(Id AS VARCHAR(4)), 4)
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceNo",
                table: "Borrowings");
        }
    }
}
