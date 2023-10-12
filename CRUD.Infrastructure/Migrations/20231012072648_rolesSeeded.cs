using System;
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
                values: new object[] {  "1", "Admin", "Admin", "27cd2d42-1625-418a-9ee5-51795d9f5048"});

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] {  "2", "User", "User", "dcec7592-6d8c-4f71-8731-d932ddf82c76" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
