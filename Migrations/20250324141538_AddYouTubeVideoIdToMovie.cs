using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieStoreMvc.Migrations
{
    public partial class AddYouTubeVideoIdToMovie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YouTubeVideoId",
                table: "Movie",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YouTubeVideoId",
                table: "Movie");
        }
    }
}
