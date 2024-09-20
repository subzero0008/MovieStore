using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieStoreMvc.Migrations
{
    public partial class AddPlaceToShowColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BroadcastingInfo",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "PlotDescription",
                table: "Movie");

            migrationBuilder.AddColumn<string>(
                name: "PlaceToShow",
                table: "Movie",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Synopsis",
                table: "Movie",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaceToShow",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "Synopsis",
                table: "Movie");

            migrationBuilder.AddColumn<string>(
                name: "BroadcastingInfo",
                table: "Movie",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlotDescription",
                table: "Movie",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
