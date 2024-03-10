using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebTool.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bingo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    consumed_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bingo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bingo_client",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    client_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    bingo_code = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    bingo_hit_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    winner_issued_by_user = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bingo_client", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    number = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "999, 1"),
                    code = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    role = table.Column<byte>(type: "tinyint", nullable: false),
                    signup_type = table.Column<byte>(type: "tinyint", nullable: false),
                    gender = table.Column<byte>(type: "tinyint", nullable: false),
                    telno = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    status = table.Column<byte>(type: "tinyint", nullable: false),
                    avatar_relative_path = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    login_failed_count = table.Column<int>(type: "int", nullable: false),
                    login_locked_until = table.Column<DateTime>(type: "datetime2", nullable: true),
                    last_login_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "avatar_relative_path", "code", "deleted_at", "email", "gender", "last_login_at", "login_failed_count", "login_locked_until", "name", "password", "role", "signup_type", "status", "telno", "updated_at" },
                values: new object[,]
                {
                    { new Guid("0932f23b-ae95-4022-af38-2edf39535ed2"), null, "vuonghuyminh", null, "vuonghuyminh.pr@gmail.com", (byte)0, null, 0, null, "Vuong Huy Minh", "Test1234!", (byte)80, (byte)0, (byte)1, null, null },
                    { new Guid("4d0e03f8-b8e8-473c-b2e8-969d0a646e8b"), null, "darkcompet", null, "darkcompet@gmail.com", (byte)0, null, 0, null, "DarkCompet", "Test1234!", (byte)100, (byte)0, (byte)1, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_bingo_code",
                table: "bingo",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bingo_client_bingo_code",
                table: "bingo_client",
                column: "bingo_code");

            migrationBuilder.CreateIndex(
                name: "IX_bingo_client_client_id",
                table: "bingo_client",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_code",
                table: "user",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_user_name",
                table: "user",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bingo");

            migrationBuilder.DropTable(
                name: "bingo_client");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
