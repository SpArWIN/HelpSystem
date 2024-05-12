using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpSystem.Migrations
{
    /// <inheritdoc />
    public partial class Mails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("a1759731-4c7b-4c3a-beb3-88c12defdc9b"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("20beea7e-573b-4de4-b74b-e91b2051f6d7"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("59b14595-d697-45ee-bee0-ff17c0c80ac1"));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("9a986be6-ec2a-44a5-9f4e-66599cd846cb"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "IsFreeZing", "IsService", "Name" },
                values: new object[] { new Guid("eb6766da-60a8-4ca5-bff0-bd5d0bd206ab"), false, true, "Склад утилизации" });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "Email", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("7200b51a-177f-4092-94b1-0e1a0ae220d7"), null, null, "nikola10wwww@mail.ru", null, null, null, new Guid("9a986be6-ec2a-44a5-9f4e-66599cd846cb") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("7200b51a-177f-4092-94b1-0e1a0ae220d7"));

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("eb6766da-60a8-4ca5-bff0-bd5d0bd206ab"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("9a986be6-ec2a-44a5-9f4e-66599cd846cb"));

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Profiles");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("59b14595-d697-45ee-bee0-ff17c0c80ac1"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "IsFreeZing", "IsService", "Name" },
                values: new object[] { new Guid("20beea7e-573b-4de4-b74b-e91b2051f6d7"), false, true, "Склад утилизации" });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("a1759731-4c7b-4c3a-beb3-88c12defdc9b"), null, null, null, null, null, new Guid("59b14595-d697-45ee-bee0-ff17c0c80ac1") });
        }
    }
}
