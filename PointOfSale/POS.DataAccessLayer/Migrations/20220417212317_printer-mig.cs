using Microsoft.EntityFrameworkCore.Migrations;

namespace POS.DataAccessLayer.Migrations
{
    public partial class printermig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseOrderOrderId",
                table: "PurchaseOrderDetails");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_SaleOrderDetails_SaleOrder_OrderId",
            //    table: "SaleOrderDetails");

            //migrationBuilder.DropColumn(
            //    name: "PurchaseOrderOrderId",
            //    table: "PurchaseOrderDetails");
            //migrationBuilder.DropColumn(
            //   name: "SaleOrderOrderId",
            //   table: "SaleOrderDetails");

            migrationBuilder.AddColumn<string>(
                name: "Printer",
                table: "Companies",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_PurchaseOrders_OrderId",
                table: "PurchaseOrderDetails",
                column: "OrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderDetails_SaleOrder_OrderId",
                table: "SaleOrderDetails",
                column: "OrderId",
                principalTable: "SaleOrder",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_PurchaseOrders_OrderId",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderDetails_SaleOrder_OrderId",
                table: "SaleOrderDetails");

            migrationBuilder.DropColumn(
                name: "Printer",
                table: "Companies");

            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "PurchaseOrderDetails",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_PurchaseOrders_OrderId",
                table: "PurchaseOrderDetails",
                column: "OrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderDetails_SaleOrder_OrderId",
                table: "SaleOrderDetails",
                column: "OrderId",
                principalTable: "SaleOrder",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
