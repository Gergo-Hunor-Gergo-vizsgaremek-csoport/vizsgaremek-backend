using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VizsgaremekBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddParentIdToPeldany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "parentid",
                table: "peldanys",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "parentid",
                table: "peldanys");
        }
    }
}
