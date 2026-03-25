using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSWP391.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreIngredientStockAndStoreCostRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CentralKitchenId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoreCostRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostType = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCostRecord", x => x.Id);
                    table.ForeignKey(
                        name: "fk_scr_ingredient",
                        column: x => x.IngredientId,
                        principalTable: "Ingredient",
                        principalColumn: "ingredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_scr_store",
                        column: x => x.StoreId,
                        principalTable: "FranchiseStore",
                        principalColumn: "store_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreIngredientStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    IngredientId = table.Column<int>(type: "int", nullable: false),
                    CurrentStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreIngredientStock", x => x.Id);
                    table.ForeignKey(
                        name: "fk_sis_ingredient",
                        column: x => x.IngredientId,
                        principalTable: "Ingredient",
                        principalColumn: "ingredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sis_store",
                        column: x => x.StoreId,
                        principalTable: "FranchiseStore",
                        principalColumn: "store_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreCostRecord_IngredientId",
                table: "StoreCostRecord",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCostRecord_StoreId",
                table: "StoreCostRecord",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreIngredientStock_IngredientId",
                table: "StoreIngredientStock",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreIngredientStock_Store",
                table: "StoreIngredientStock",
                columns: new[] { "StoreId", "IngredientId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreCostRecord");

            migrationBuilder.DropTable(
                name: "StoreIngredientStock");

            migrationBuilder.DropColumn(
                name: "CentralKitchenId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "AspNetUsers");
        }
    }
}
