using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDev.Inventory.API.Migrations
{
    /// <inheritdoc />
    public partial class AlterVersionCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                schema: "sd_inventory",
                table: "Spu",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                schema: "sd_inventory",
                table: "Spu");
        }
    }
}
