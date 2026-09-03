using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarangayEquipmentAPI.Migrations
{
    /// <inheritdoc />
    public partial class MultiEquipmentBorrowing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_Equipments_EquipmentId",
                table: "Borrowings");

            migrationBuilder.DropIndex(
                name: "IX_Borrowings_EquipmentId",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "QuantityBorrowed",
                table: "Borrowings");

            migrationBuilder.CreateTable(
                name: "BorrowingItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BorrowingId = table.Column<int>(type: "int", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    QuantityBorrowed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowingItems_Borrowings_BorrowingId",
                        column: x => x.BorrowingId,
                        principalTable: "Borrowings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowingItems_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowingItems_BorrowingId",
                table: "BorrowingItems",
                column: "BorrowingId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowingItems_EquipmentId",
                table: "BorrowingItems",
                column: "EquipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowingItems");

            migrationBuilder.AddColumn<int>(
                name: "EquipmentId",
                table: "Borrowings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantityBorrowed",
                table: "Borrowings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_EquipmentId",
                table: "Borrowings",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_Equipments_EquipmentId",
                table: "Borrowings",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
