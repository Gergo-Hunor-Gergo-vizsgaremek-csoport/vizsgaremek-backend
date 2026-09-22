using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizsgaremekBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddSelejtFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ishibas",
                table: "peldanys",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isselejt",
                table: "peldanys",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isselejtsugg",
                table: "peldanys",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "selejteddate",
                table: "peldanys",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ishibas",
                table: "peldanys");

            migrationBuilder.DropColumn(
                name: "isselejt",
                table: "peldanys");

            migrationBuilder.DropColumn(
                name: "isselejtsugg",
                table: "peldanys");

            migrationBuilder.DropColumn(
                name: "selejteddate",
                table: "peldanys");
        }
    }
}
