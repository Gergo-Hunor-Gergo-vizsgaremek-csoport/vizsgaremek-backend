using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VizsgaremekBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_kolcsonzess_users_userid",
                table: "kolcsonzess");

            migrationBuilder.DropForeignKey(
                name: "fk_logs_users_userid",
                table: "logs");

            migrationBuilder.DropForeignKey(
                name: "fk_peldanys_users_felelosid",
                table: "peldanys");

            migrationBuilder.DropForeignKey(
                name: "fk_rendeleses_users_userid",
                table: "rendeleses");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "accessfailedcount",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "concurrencystamp",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "emailconfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "lockoutenabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "lockoutend",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalizedemail",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalizedusername",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "passwordhash",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phonenumber",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "phonenumberconfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "securitystamp",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "twofactorenabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_aspnetusers",
                table: "AspNetUsers",
                column: "id");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedname = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrencystamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetroles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    claimtype = table.Column<string>(type: "text", nullable: true),
                    claimvalue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_aspnetuserclaims_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    loginprovider = table.Column<string>(type: "text", nullable: false),
                    providerkey = table.Column<string>(type: "text", nullable: false),
                    providerdisplayname = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserlogins", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "fk_aspnetuserlogins_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    loginprovider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetusertokens", x => new { x.userid, x.loginprovider, x.name });
                    table.ForeignKey(
                        name: "fk_aspnetusertokens_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<Guid>(type: "uuid", nullable: false),
                    claimtype = table.Column<string>(type: "text", nullable: true),
                    claimvalue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetroleclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_aspnetroleclaims_aspnetroles_roleid",
                        column: x => x.roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    roleid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserroles", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_aspnetuserroles_aspnetroles_roleid",
                        column: x => x.roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_aspnetuserroles_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "normalizedemail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "normalizedusername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnetroleclaims_roleid",
                table: "AspNetRoleClaims",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalizedname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnetuserclaims_userid",
                table: "AspNetUserClaims",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_aspnetuserlogins_userid",
                table: "AspNetUserLogins",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_aspnetuserroles_roleid",
                table: "AspNetUserRoles",
                column: "roleid");

            migrationBuilder.AddForeignKey(
                name: "fk_kolcsonzess_user_userid",
                table: "kolcsonzess",
                column: "userid",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_logs_user_userid",
                table: "logs",
                column: "userid",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_peldanys_user_felelosid",
                table: "peldanys",
                column: "felelosid",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rendeleses_user_userid",
                table: "rendeleses",
                column: "userid",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_kolcsonzess_user_userid",
                table: "kolcsonzess");

            migrationBuilder.DropForeignKey(
                name: "fk_logs_user_userid",
                table: "logs");

            migrationBuilder.DropForeignKey(
                name: "fk_peldanys_user_felelosid",
                table: "peldanys");

            migrationBuilder.DropForeignKey(
                name: "fk_rendeleses_user_userid",
                table: "rendeleses");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_aspnetusers",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "accessfailedcount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "concurrencystamp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "emailconfirmed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "lockoutenabled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "lockoutend",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "normalizedemail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "normalizedusername",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "passwordhash",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "phonenumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "phonenumberconfirmed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "securitystamp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "twofactorenabled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "username",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "users");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_kolcsonzess_users_userid",
                table: "kolcsonzess",
                column: "userid",
                principalTable: "users",
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
                name: "fk_peldanys_users_felelosid",
                table: "peldanys",
                column: "felelosid",
                principalTable: "users",
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
    }
}
