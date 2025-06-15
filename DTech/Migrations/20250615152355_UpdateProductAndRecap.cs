using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTech.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductAndRecap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InitialCost",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialCost",
                table: "OrderProducts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MonthlyRecaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Profit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLaptops = table.Column<int>(type: "int", nullable: false),
                    TotalSmartPhone = table.Column<int>(type: "int", nullable: false),
                    TotalTablet = table.Column<int>(type: "int", nullable: false),
                    TotalAccessory = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyRecaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisitorCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorCounts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyRecaps");

            migrationBuilder.DropTable(
                name: "VisitorCounts");

            migrationBuilder.DropColumn(
                name: "InitialCost",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InitialCost",
                table: "OrderProducts");
        }
    }
}
