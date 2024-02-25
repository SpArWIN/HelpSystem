using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpSystem.Migrations
{
    /// <inheritdoc />
    public partial class datas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("54bbbc0c-7762-48b2-a282-b45814497977"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f8fbb12b-aca6-43f9-b31c-1b7f0fb53da8"));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("e63f5f08-c24e-417e-9aaf-ddfd0132c9df"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("54adf5b6-c5a4-45ea-b1d6-bca50400c9ea"), null, null, null, null, null, new Guid("e63f5f08-c24e-417e-9aaf-ddfd0132c9df") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("54adf5b6-c5a4-45ea-b1d6-bca50400c9ea"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e63f5f08-c24e-417e-9aaf-ddfd0132c9df"));

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Profiles");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("f8fbb12b-aca6-43f9-b31c-1b7f0fb53da8"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Surname", "UserId" },
                values: new object[] { new Guid("54bbbc0c-7762-48b2-a282-b45814497977"), null, null, null, null, new Guid("f8fbb12b-aca6-43f9-b31c-1b7f0fb53da8") });
        }
    }
}
