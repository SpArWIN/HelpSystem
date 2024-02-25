using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("21421014-8e4d-4423-aa90-0271fd0f300f"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6aac60a2-25c8-4827-9527-5816ef7d28d1"));

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("f8fbb12b-aca6-43f9-b31c-1b7f0fb53da8"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Surname", "UserId" },
                values: new object[] { new Guid("54bbbc0c-7762-48b2-a282-b45814497977"), null, null, null, null, new Guid("f8fbb12b-aca6-43f9-b31c-1b7f0fb53da8") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("54bbbc0c-7762-48b2-a282-b45814497977"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f8fbb12b-aca6-43f9-b31c-1b7f0fb53da8"));

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Profiles");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("6aac60a2-25c8-4827-9527-5816ef7d28d1"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "UserId" },
                values: new object[] { new Guid("21421014-8e4d-4423-aa90-0271fd0f300f"), null, null, new Guid("6aac60a2-25c8-4827-9527-5816ef7d28d1") });
        }
    }
}
