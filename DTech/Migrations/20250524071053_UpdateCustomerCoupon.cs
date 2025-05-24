using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTech.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "CustomerCoupons");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CustomerCoupons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "CustomerCoupons",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CustomerCoupons",
                type: "int",
                nullable: true);
        }
    }
}
