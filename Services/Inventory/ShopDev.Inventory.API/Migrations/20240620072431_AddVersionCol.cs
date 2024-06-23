using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDev.Inventory.API.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Version",
                schema: "sd_inventory",
                table: "Spu",
                type: "bigint",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldRowVersion: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "sd_inventory",
                table: "Spu",
                type: "int",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldRowVersion: true);
        }
    }
}
