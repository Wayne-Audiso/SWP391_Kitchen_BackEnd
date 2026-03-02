using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSWP391.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientFieldsAndRecipeIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "minStock",
                table: "Ingredient",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "price",
                table: "Ingredient",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecipeIngredient",
                columns: table => new
                {
                    recipeIngredientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recipeID = table.Column<int>(type: "int", nullable: false),
                    ingredientID = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredient", x => x.recipeIngredientID);
                    table.ForeignKey(
                        name: "fk_ri_ingredient",
                        column: x => x.ingredientID,
                        principalTable: "Ingredient",
                        principalColumn: "ingredientID");
                    table.ForeignKey(
                        name: "fk_ri_recipe",
                        column: x => x.recipeID,
                        principalTable: "Recipe",
                        principalColumn: "recipeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredient_ingredientID",
                table: "RecipeIngredient",
                column: "ingredientID");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredient_recipeID",
                table: "RecipeIngredient",
                column: "recipeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeIngredient");

            migrationBuilder.DropColumn(
                name: "minStock",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "price",
                table: "Ingredient");
        }
    }
}
