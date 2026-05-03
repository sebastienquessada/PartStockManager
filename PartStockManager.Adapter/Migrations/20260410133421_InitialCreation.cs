using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartStockManager.Adapter.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    LowStockThreshold = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                    table.CheckConstraint("CK_Part_LowStockThreshold_GTE_Zero", "LowStockThreshold >= 0");
                    table.CheckConstraint("CK_Part_Name_MinLength", "LEN(Name) >= 1");
                    table.CheckConstraint("CK_Part_Reference_MinLength", "LEN(Reference) >= 1");
                    table.CheckConstraint("CK_Part_StockQuantity_GTE_Zero", "StockQuantity >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parts_Reference",
                table: "Parts",
                column: "Reference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parts");
        }
    }
}
