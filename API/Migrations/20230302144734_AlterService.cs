using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AlterService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("dcc3e668-3b7c-4285-bfd1-072c1805aadd"));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Services",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceType",
                table: "Services",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "UserId",
                keyValue: new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"),
                column: "Password",
                value: "$2a$11$mge9nfB2klcqX1LLWryuJ.vb9Vb.UNt/Y2j6poon8IUs2sMqcMGXC");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "AccountUserId", "Name" },
                values: new object[] { new Guid("78a979fe-2ffc-44ee-9390-2162f00ee105"), new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"), "Администратор" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"),
                columns: new[] { "BirthDate", "First_name", "Last_name" },
                values: new object[] { new DateTime(1993, 3, 2, 17, 47, 34, 444, DateTimeKind.Local).AddTicks(6807), "Админ", "Админ" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("78a979fe-2ffc-44ee-9390-2162f00ee105"));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Services",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceType",
                table: "Services",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "UserId",
                keyValue: new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"),
                column: "Password",
                value: "$2a$11$h63pckLLqAIvRV4soHvo8e02WmHj.8BC6tiRZiE..cVmaGFODRxXq");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "AccountUserId", "Name" },
                values: new object[] { new Guid("dcc3e668-3b7c-4285-bfd1-072c1805aadd"), new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"), "Администратор" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"),
                columns: new[] { "BirthDate", "First_name", "Last_name" },
                values: new object[] { new DateTime(1993, 2, 27, 4, 51, 1, 743, DateTimeKind.Local).AddTicks(246), null, null });
        }
    }
}
