using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpSystem.Migrations
{
    /// <inheritdoc />
    public partial class FreeZing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("8ada6c6a-7084-4300-a170-3832d6b0839a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0f86ed13-e491-437f-8737-08cadb4ca0fd"));

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeZing",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "TransferProducts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeZing",
                table: "Providers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("0ce3a5ef-dadb-467e-a296-630d288e28e9"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("dd3b68b9-a2ce-495c-a646-2e157b1808d1"), null, null, null, null, null, new Guid("0ce3a5ef-dadb-467e-a296-630d288e28e9") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("dd3b68b9-a2ce-495c-a646-2e157b1808d1"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0ce3a5ef-dadb-467e-a296-630d288e28e9"));

            migrationBuilder.DropColumn(
                name: "IsFreeZing",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "IsFreeZing",
                table: "Providers");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "TransferProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("0f86ed13-e491-437f-8737-08cadb4ca0fd"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("8ada6c6a-7084-4300-a170-3832d6b0839a"), null, null, null, null, null, new Guid("0f86ed13-e491-437f-8737-08cadb4ca0fd") });
        }
    }
}
