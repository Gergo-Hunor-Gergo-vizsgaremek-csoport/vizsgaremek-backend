using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizsgaremekBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToKolcsonzes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_kolcsonzess_userid",
                table: "kolcsonzess",
                column: "userid");

            migrationBuilder.AddForeignKey(
                name: "fk_kolcsonzess_users_userid",
                table: "kolcsonzess",
                column: "userid",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_kolcsonzess_users_userid",
                table: "kolcsonzess");

            migrationBuilder.DropIndex(
                name: "ix_kolcsonzess_userid",
                table: "kolcsonzess");
        }
    }
}
