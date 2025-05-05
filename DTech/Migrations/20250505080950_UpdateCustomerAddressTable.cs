using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTech.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerAddressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "CustomerAddresses",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "CustomerAddresses");
        }
    }
}
