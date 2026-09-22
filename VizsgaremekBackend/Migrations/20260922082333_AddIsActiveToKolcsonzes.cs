using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizsgaremekBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToKolcsonzes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isactive",
                table: "kolcsonzess",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isactive",
                table: "kolcsonzess");
        }
    }
}
