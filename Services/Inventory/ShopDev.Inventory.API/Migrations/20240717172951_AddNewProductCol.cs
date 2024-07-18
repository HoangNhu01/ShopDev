using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDev.Inventory.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProductCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                schema: "sd_inventory",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SetFeaturedDate",
                schema: "sd_inventory",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                schema: "sd_inventory",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SetFeaturedDate",
                schema: "sd_inventory",
                table: "Product");
        }
    }
}
