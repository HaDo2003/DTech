using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTech.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderTable3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingDistrictId",
                table: "Orders",
                column: "ShippingDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingProvinceId",
                table: "Orders",
                column: "ShippingProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingWardId",
                table: "Orders",
                column: "ShippingWardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Districts_ShippingDistrictId",
                table: "Orders",
                column: "ShippingDistrictId",
                principalTable: "Districts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Provinces_ShippingProvinceId",
                table: "Orders",
                column: "ShippingProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Wards_ShippingWardId",
                table: "Orders",
                column: "ShippingWardId",
                principalTable: "Wards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Districts_ShippingDistrictId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Provinces_ShippingProvinceId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Wards_ShippingWardId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingDistrictId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingProvinceId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingWardId",
                table: "Orders");
        }
    }
}
