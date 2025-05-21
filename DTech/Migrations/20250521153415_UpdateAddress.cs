using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTech.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "CustomerAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "CustomerAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "CustomerAddresses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "District",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "CustomerAddresses");
        }
    }
}
