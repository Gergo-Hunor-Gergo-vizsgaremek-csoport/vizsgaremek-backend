using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizsgaremekBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_peldanys_felelosid",
                table: "peldanys",
                column: "felelosid");

            migrationBuilder.CreateIndex(
                name: "ix_peldanys_locationid",
                table: "peldanys",
                column: "locationid");

            migrationBuilder.CreateIndex(
                name: "ix_peldanys_parentid",
                table: "peldanys",
                column: "parentid");

            migrationBuilder.CreateIndex(
                name: "ix_peldanys_typeid",
                table: "peldanys",
                column: "typeid");

            migrationBuilder.CreateIndex(
                name: "ix_logs_userid",
                table: "logs",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_kolcsonzess_peldanyid",
                table: "kolcsonzess",
                column: "peldanyid");

            migrationBuilder.AddForeignKey(
                name: "fk_kolcsonzess_peldanys_peldanyid",
                table: "kolcsonzess",
                column: "peldanyid",
                principalTable: "peldanys",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_logs_users_userid",
                table: "logs",
                column: "userid",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_peldanys_locations_locationid",
                table: "peldanys",
                column: "locationid",
                principalTable: "locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_peldanys_peldanys_parentid",
                table: "peldanys",
                column: "parentid",
                principalTable: "peldanys",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_peldanys_types_typeid",
                table: "peldanys",
                column: "typeid",
                principalTable: "types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_peldanys_users_felelosid",
                table: "peldanys",
                column: "felelosid",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_kolcsonzess_peldanys_peldanyid",
                table: "kolcsonzess");

            migrationBuilder.DropForeignKey(
                name: "fk_logs_users_userid",
                table: "logs");

            migrationBuilder.DropForeignKey(
                name: "fk_peldanys_locations_locationid",
                table: "peldanys");

            migrationBuilder.DropForeignKey(
                name: "fk_peldanys_peldanys_parentid",
                table: "peldanys");

            migrationBuilder.DropForeignKey(
                name: "fk_peldanys_types_typeid",
                table: "peldanys");

            migrationBuilder.DropForeignKey(
                name: "fk_peldanys_users_felelosid",
                table: "peldanys");

            migrationBuilder.DropIndex(
                name: "ix_peldanys_felelosid",
                table: "peldanys");

            migrationBuilder.DropIndex(
                name: "ix_peldanys_locationid",
                table: "peldanys");

            migrationBuilder.DropIndex(
                name: "ix_peldanys_parentid",
                table: "peldanys");

            migrationBuilder.DropIndex(
                name: "ix_peldanys_typeid",
                table: "peldanys");

            migrationBuilder.DropIndex(
                name: "ix_logs_userid",
                table: "logs");

            migrationBuilder.DropIndex(
                name: "ix_kolcsonzess_peldanyid",
                table: "kolcsonzess");
        }
    }
}
