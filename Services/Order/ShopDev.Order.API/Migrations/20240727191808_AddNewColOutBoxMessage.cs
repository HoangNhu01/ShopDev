using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDev.Order.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColOutBoxMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLock",
                schema: "sd_order",
                table: "OutboxMessage",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "sd_order",
                table: "OrderGen",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("ca6a15be-51f3-4b89-8fa7-7493b19147d1"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("9c1e825a-9280-47cc-a5b0-5d0aa62a645b"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLock",
                schema: "sd_order",
                table: "OutboxMessage");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "sd_order",
                table: "OrderGen",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("9c1e825a-9280-47cc-a5b0-5d0aa62a645b"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("ca6a15be-51f3-4b89-8fa7-7493b19147d1"));
        }
    }
}
