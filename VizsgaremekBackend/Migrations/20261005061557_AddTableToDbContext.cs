using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizsgaremekBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTableToDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rendeles_types_typeid",
                table: "rendeles");

            migrationBuilder.DropForeignKey(
                name: "fk_rendeles_users_userid",
                table: "rendeles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rendeles",
                table: "rendeles");

            migrationBuilder.RenameTable(
                name: "rendeles",
                newName: "rendeleses");

            migrationBuilder.RenameIndex(
                name: "ix_rendeles_userid",
                table: "rendeleses",
                newName: "ix_rendeleses_userid");

            migrationBuilder.RenameIndex(
                name: "ix_rendeles_typeid",
                table: "rendeleses",
                newName: "ix_rendeleses_typeid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rendeleses",
                table: "rendeleses",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_rendeleses_types_typeid",
                table: "rendeleses",
                column: "typeid",
                principalTable: "types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rendeleses_users_userid",
                table: "rendeleses",
                column: "userid",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rendeleses_types_typeid",
                table: "rendeleses");

            migrationBuilder.DropForeignKey(
                name: "fk_rendeleses_users_userid",
                table: "rendeleses");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rendeleses",
                table: "rendeleses");

            migrationBuilder.RenameTable(
                name: "rendeleses",
                newName: "rendeles");

            migrationBuilder.RenameIndex(
                name: "ix_rendeleses_userid",
                table: "rendeles",
                newName: "ix_rendeles_userid");

            migrationBuilder.RenameIndex(
                name: "ix_rendeleses_typeid",
                table: "rendeles",
                newName: "ix_rendeles_typeid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rendeles",
                table: "rendeles",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_rendeles_types_typeid",
                table: "rendeles",
                column: "typeid",
                principalTable: "types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rendeles_users_userid",
                table: "rendeles",
                column: "userid",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
