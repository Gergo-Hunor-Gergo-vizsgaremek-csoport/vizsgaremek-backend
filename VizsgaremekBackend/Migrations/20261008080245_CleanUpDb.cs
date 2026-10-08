using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizsgaremekBackend.Migrations
{
    /// <inheritdoc />
    public partial class CleanUpDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isactive",
                table: "kolcsonzess");

            migrationBuilder.DropColumn(
                name: "isdeviceadmin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "issysadmin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "isuseradmin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "name",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "activekolcsonzesid",
                table: "peldanys",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateIndex(
                name: "ix_peldanys_activekolcsonzesid",
                table: "peldanys",
                column: "activekolcsonzesid");

            migrationBuilder.AddForeignKey(
                name: "fk_peldanys_kolcsonzess_activekolcsonzesid",
                table: "peldanys",
                column: "activekolcsonzesid",
                principalTable: "kolcsonzess",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_peldanys_kolcsonzess_activekolcsonzesid",
                table: "peldanys");

            migrationBuilder.DropIndex(
                name: "ix_peldanys_activekolcsonzesid",
                table: "peldanys");

            migrationBuilder.DropColumn(
                name: "activekolcsonzesid",
                table: "peldanys");

            migrationBuilder.AddColumn<bool>(
                name: "isactive",
                table: "kolcsonzess",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isdeviceadmin",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "issysadmin",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isuseradmin",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
