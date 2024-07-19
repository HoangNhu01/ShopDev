using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDev.Order.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "sd_order");

            migrationBuilder.CreateTable(
                name: "OrderGen",
                schema: "sd_order",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false,
                        defaultValue: new Guid("c3b359b1-9d42-482d-adc7-d493963258cc")
                    ),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ShipName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShipAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderGen", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                schema: "sd_order",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SpuId = table.Column<int>(type: "int", nullable: false),
                    StockStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Product = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetail_OrderGen_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "sd_order",
                        principalTable: "OrderGen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "OrderProcess",
                schema: "sd_order",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProcessStatus = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProcess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProcess_OrderGen_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "sd_order",
                        principalTable: "OrderGen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail",
                schema: "sd_order",
                table: "OrderDetail",
                columns: new[] { "ProductId", "SpuId", "Deleted" },
                descending: new bool[0]
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderId",
                schema: "sd_order",
                table: "OrderDetail",
                column: "OrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderGen",
                schema: "sd_order",
                table: "OrderGen",
                columns: new[]
                {
                    "OrderDate",
                    "UserId",
                    "Deleted",
                    "ShipName",
                    "PaymentStatus",
                    "Status"
                },
                descending: new bool[0]
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderProcess",
                schema: "sd_order",
                table: "OrderProcess",
                columns: new[] { "WarehouseName", "ProcessStatus" },
                descending: new bool[0]
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderProcess_OrderId",
                schema: "sd_order",
                table: "OrderProcess",
                column: "OrderId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OrderDetail", schema: "sd_order");

            migrationBuilder.DropTable(name: "OrderProcess", schema: "sd_order");

            migrationBuilder.DropTable(name: "OrderGen", schema: "sd_order");
        }
    }
}
