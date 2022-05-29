using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace POS.DataAccessLayer.Migrations
{
    public partial class Cust_Sup_transaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnOrders_Customers_CustomerId",
                table: "ReturnOrders");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "SaleOrderDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "SaleOrder",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "ReturnOrders",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "CustomerTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleOrderId = table.Column<long>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    Credit = table.Column<decimal>(nullable: false),
                    Debit = table.Column<decimal>(nullable: false),
                    CreateBy = table.Column<string>(maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    CreateAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerTransactions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerTransactions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SupplierTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<long>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    Credit = table.Column<decimal>(nullable: false),
                    Debit = table.Column<decimal>(nullable: false),
                    CreateBy = table.Column<string>(maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    CreateAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierTransactions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierTransactions_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransactions_CompanyId",
                table: "CustomerTransactions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransactions_CustomerId",
                table: "CustomerTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierTransactions_CompanyId",
                table: "SupplierTransactions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierTransactions_SupplierId",
                table: "SupplierTransactions",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnOrders_Customers_CustomerId",
                table: "ReturnOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnOrders_Customers_CustomerId",
                table: "ReturnOrders");

            migrationBuilder.DropTable(
                name: "CustomerTransactions");

            migrationBuilder.DropTable(
                name: "SupplierTransactions");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "SaleOrderDetails");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "SaleOrder");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "ReturnOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnOrders_Customers_CustomerId",
                table: "ReturnOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
