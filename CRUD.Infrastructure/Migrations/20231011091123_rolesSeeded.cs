using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRUD.Infrastructure.Migrations
{
    public partial class rolesSeeded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { "9a96fc49-ca7d-4612-9434-b6716e04869a", "Admin", "Admin", "1" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { "a6cfe954-a12d-4ac4-a8f7-910c47cb884c", "User", "User", "2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "9a96fc49-ca7d-4612-9434-b6716e04869a");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "a6cfe954-a12d-4ac4-a8f7-910c47cb884c");
        }
    }
}
