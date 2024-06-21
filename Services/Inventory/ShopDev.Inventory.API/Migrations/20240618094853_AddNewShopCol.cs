using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDev.Inventory.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewShopCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                schema: "sd_inventory",
                table: "Shop");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                schema: "sd_inventory",
                table: "Shop",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                schema: "sd_inventory",
                table: "Shop");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                schema: "sd_inventory",
                table: "Shop",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
