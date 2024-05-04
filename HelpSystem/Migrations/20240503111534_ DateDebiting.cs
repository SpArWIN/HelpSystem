using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpSystem.Migrations
{
    /// <inheritdoc />
    public partial class DateDebiting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("3471e5c2-43dd-4753-a637-5e4dbad68ec4"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("b38c0712-2c21-4a58-ba90-28dab36a0ac4"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d6013b98-bd5b-4d24-98bc-22c38813df9e"));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeDebbiting",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("e0d220e5-ba50-480b-a82f-af86e52acafd"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "IsFreeZing", "IsService", "Name" },
                values: new object[] { new Guid("1a9f5e0b-38d3-428b-9091-d2d35a4baec5"), false, true, "Склад утилизации" });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("f9a1ded2-237f-4d1a-b8da-6a7c93b17b3f"), null, null, null, null, null, new Guid("e0d220e5-ba50-480b-a82f-af86e52acafd") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("f9a1ded2-237f-4d1a-b8da-6a7c93b17b3f"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("1a9f5e0b-38d3-428b-9091-d2d35a4baec5"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e0d220e5-ba50-480b-a82f-af86e52acafd"));

            migrationBuilder.DropColumn(
                name: "TimeDebbiting",
                table: "Products");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("d6013b98-bd5b-4d24-98bc-22c38813df9e"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "IsFreeZing", "IsService", "Name" },
                values: new object[] { new Guid("b38c0712-2c21-4a58-ba90-28dab36a0ac4"), false, true, "Склад утилизации" });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("3471e5c2-43dd-4753-a637-5e4dbad68ec4"), null, null, null, null, null, new Guid("d6013b98-bd5b-4d24-98bc-22c38813df9e") });
        }
    }
}
