using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSWP391.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTokenFromModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Column Token already does not exist in Database (or was manually handled).
            // migrationBuilder.DropColumn(
            //     name: "Token",
            //     table: "UserInfo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "UserInfo",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

