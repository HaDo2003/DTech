using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTech.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAddress2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "CustomerAddresses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceId",
                table: "CustomerAddresses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "CustomerAddresses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wards_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DistrictId",
                table: "Orders",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProvinceId",
                table: "Orders",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WardId",
                table: "Orders",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_DistrictId",
                table: "CustomerAddresses",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_ProvinceId",
                table: "CustomerAddresses",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_WardId",
                table: "CustomerAddresses",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceId",
                table: "Districts",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_DistrictId",
                table: "Wards",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_Districts_DistrictId",
                table: "CustomerAddresses",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_Provinces_ProvinceId",
                table: "CustomerAddresses",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_Wards_WardId",
                table: "CustomerAddresses",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Districts_DistrictId",
                table: "Orders",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Provinces_ProvinceId",
                table: "Orders",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Wards_WardId",
                table: "Orders",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_Districts_DistrictId",
                table: "CustomerAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_Provinces_ProvinceId",
                table: "CustomerAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_Wards_WardId",
                table: "CustomerAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Districts_DistrictId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Provinces_ProvinceId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Wards_WardId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DistrictId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ProvinceId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_WardId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddresses_DistrictId",
                table: "CustomerAddresses");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddresses_ProvinceId",
                table: "CustomerAddresses");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAddresses_WardId",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "CustomerAddresses");

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
    }
}
