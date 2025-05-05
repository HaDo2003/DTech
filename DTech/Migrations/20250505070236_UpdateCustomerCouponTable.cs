using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTech.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerCouponTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCoupons_AspNetUsers_CustomerId1",
                table: "CustomerCoupons");

            migrationBuilder.DropIndex(
                name: "IX_CustomerCoupons_CustomerId1",
                table: "CustomerCoupons");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "CustomerCoupons");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "CustomerCoupons",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCoupons_CustomerId",
                table: "CustomerCoupons",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCoupons_AspNetUsers_CustomerId",
                table: "CustomerCoupons",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCoupons_AspNetUsers_CustomerId",
                table: "CustomerCoupons");

            migrationBuilder.DropIndex(
                name: "IX_CustomerCoupons_CustomerId",
                table: "CustomerCoupons");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CustomerCoupons",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId1",
                table: "CustomerCoupons",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCoupons_CustomerId1",
                table: "CustomerCoupons",
                column: "CustomerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCoupons_AspNetUsers_CustomerId1",
                table: "CustomerCoupons",
                column: "CustomerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
