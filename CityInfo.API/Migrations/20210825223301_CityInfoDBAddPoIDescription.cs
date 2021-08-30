using Microsoft.EntityFrameworkCore.Migrations;

namespace CityInfo.API.Migrations
{
    public partial class CityInfoDBAddPoIDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PointsOfInterests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PointsOfInterests");
        }
    }
}
