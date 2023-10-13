using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRUD.Infrastructure.Migrations
{
    public partial class todoUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
    

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Todos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
