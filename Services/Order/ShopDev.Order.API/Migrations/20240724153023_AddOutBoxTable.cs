using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDev.Order.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOutBoxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "sd_order",
                table: "OrderGen",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("452ff95e-77d6-4f00-8ecd-bec7820c7385"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("c3b359b1-9d42-482d-adc7-d493963258cc"));

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                schema: "sd_order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Event = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage",
                schema: "sd_order",
                table: "OutboxMessage",
                column: "ProcessedOnUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessage",
                schema: "sd_order");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "sd_order",
                table: "OrderGen",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("c3b359b1-9d42-482d-adc7-d493963258cc"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValue: new Guid("452ff95e-77d6-4f00-8ecd-bec7820c7385"));
        }
    }
}
