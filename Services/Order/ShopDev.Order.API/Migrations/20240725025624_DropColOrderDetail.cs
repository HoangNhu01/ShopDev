using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDev.Order.API.Migrations
{
    /// <inheritdoc />
    public partial class DropColOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderDetail",
                schema: "sd_order",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "SpuId",
                schema: "sd_order",
                table: "OrderDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "sd_order",
                table: "OrderGen",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("9c1e825a-9280-47cc-a5b0-5d0aa62a645b"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("452ff95e-77d6-4f00-8ecd-bec7820c7385"));

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail",
                schema: "sd_order",
                table: "OrderDetail",
                columns: new[] { "ProductId", "Deleted" },
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderDetail",
                schema: "sd_order",
                table: "OrderDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "sd_order",
                table: "OrderGen",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("452ff95e-77d6-4f00-8ecd-bec7820c7385"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("9c1e825a-9280-47cc-a5b0-5d0aa62a645b"));

            migrationBuilder.AddColumn<int>(
                name: "SpuId",
                schema: "sd_order",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail",
                schema: "sd_order",
                table: "OrderDetail",
                columns: new[] { "ProductId", "SpuId", "Deleted" },
                descending: new bool[0]);
        }
    }
}
