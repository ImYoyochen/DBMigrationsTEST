using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MigrationsTEST.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Department", "Email", "HireDate", "Name", "Salary" },
                values: new object[,]
                {
                    { 1, "IT部門", "zhang.san@company.com", new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "張三", 50000m },
                    { 2, "人力資源部", "li.si@company.com", new DateTime(2023, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "李四", 45000m },
                    { 3, "財務部", "wang.wu@company.com", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "王五", 48000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
