using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieStoreMvc.Migrations
{
    public partial class SetDefaultName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
     
              name: "Name",
     
              table: "AspNetUsers",
      
              nullable: false,
      
              defaultValue: "Default Name", // Задаване на стойност по подразбиране
      
              oldClrType: typeof(string),
      
              oldNullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Връщаме стойността на колоната 'Name' в старото състояние (ако е необходимо)
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: false);
        }
    }
}
