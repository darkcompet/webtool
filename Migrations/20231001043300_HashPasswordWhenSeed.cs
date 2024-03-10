using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebTool.Migrations
{
    /// <inheritdoc />
    public partial class HashPasswordWhenSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user",
                keyColumn: "id",
                keyValue: new Guid("0932f23b-ae95-4022-af38-2edf39535ed2"));

            migrationBuilder.DeleteData(
                table: "user",
                keyColumn: "id",
                keyValue: new Guid("4d0e03f8-b8e8-473c-b2e8-969d0a646e8b"));

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "avatar_relative_path", "code", "deleted_at", "email", "gender", "last_login_at", "login_failed_count", "login_locked_until", "name", "password", "role", "signup_type", "status", "telno", "updated_at" },
                values: new object[,]
                {
                    { new Guid("5dd08311-6089-4638-bacf-47576ccfe23a"), null, "vuonghuyminh", null, "vuonghuyminh.pr@gmail.com", (byte)0, null, 0, null, "Vuong Huy Minh", "AQAAAAIAAYagAAAAENPLixdsBs75mkfx3MXF6Ix7e2fvrR2+xDhP6B8LS7QoAqO4bQAfoZteUtVuAsZZhg==", (byte)80, (byte)0, (byte)1, null, null },
                    { new Guid("f762944d-ba7a-4c1f-92cf-b02f524f5ffa"), null, "darkcompet", null, "darkcompet@gmail.com", (byte)0, null, 0, null, "DarkCompet", "AQAAAAIAAYagAAAAEIJbv9VNBQeknNK3YMRnCei/miP16KrmzKrUB3ZzI5EiQTY+tX+44aQugwPhb/5Rng==", (byte)100, (byte)0, (byte)1, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user",
                keyColumn: "id",
                keyValue: new Guid("5dd08311-6089-4638-bacf-47576ccfe23a"));

            migrationBuilder.DeleteData(
                table: "user",
                keyColumn: "id",
                keyValue: new Guid("f762944d-ba7a-4c1f-92cf-b02f524f5ffa"));

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "avatar_relative_path", "code", "deleted_at", "email", "gender", "last_login_at", "login_failed_count", "login_locked_until", "name", "password", "role", "signup_type", "status", "telno", "updated_at" },
                values: new object[,]
                {
                    { new Guid("0932f23b-ae95-4022-af38-2edf39535ed2"), null, "vuonghuyminh", null, "vuonghuyminh.pr@gmail.com", (byte)0, null, 0, null, "Vuong Huy Minh", "Test1234!", (byte)80, (byte)0, (byte)1, null, null },
                    { new Guid("4d0e03f8-b8e8-473c-b2e8-969d0a646e8b"), null, "darkcompet", null, "darkcompet@gmail.com", (byte)0, null, 0, null, "DarkCompet", "Test1234!", (byte)100, (byte)0, (byte)1, null, null }
                });
        }
    }
}
