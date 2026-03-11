using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSWP391.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreOrderLines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreOrderLine",
                columns: table => new
                {
                    storeOrderLineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    storeOrderID = table.Column<int>(type: "int", nullable: false),
                    productID = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StoreOrdLine", x => x.storeOrderLineID);
                    table.ForeignKey(
                        name: "fk_orderline_order",
                        column: x => x.storeOrderID,
                        principalTable: "StoreOrder",
                        principalColumn: "storeOrderID");
                    table.ForeignKey(
                        name: "fk_orderline_product",
                        column: x => x.productID,
                        principalTable: "Product",
                        principalColumn: "productID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreOrderLine_productID",
                table: "StoreOrderLine",
                column: "productID");

            migrationBuilder.CreateIndex(
                name: "IX_StoreOrderLine_storeOrderID",
                table: "StoreOrderLine",
                column: "storeOrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreOrderLine");
        }
    }
}
