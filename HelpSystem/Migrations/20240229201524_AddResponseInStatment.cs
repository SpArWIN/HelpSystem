using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddResponseInStatment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("c2ab95c2-8813-495d-89da-e7eea3f64e3c"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aef76bfd-22fa-465a-a62c-4706c62bc2cc"));

            migrationBuilder.AddColumn<string>(
                name: "ResponseAnswer",
                table: "Statements",
                nullable: true); 


            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("8b7576b1-cd69-449c-a099-4026c96f9843"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("1f4bf4b7-cefc-4ddc-a220-d3714bb8ef18"), null, null, null, null, null, new Guid("8b7576b1-cd69-449c-a099-4026c96f9843") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("1f4bf4b7-cefc-4ddc-a220-d3714bb8ef18"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8b7576b1-cd69-449c-a099-4026c96f9843"));

            migrationBuilder.RenameColumn(
                name: "ResponseAnswer",
                table: "Statements",
                newName: "Response");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("aef76bfd-22fa-465a-a62c-4706c62bc2cc"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("c2ab95c2-8813-495d-89da-e7eea3f64e3c"), null, null, null, null, null, new Guid("aef76bfd-22fa-465a-a62c-4706c62bc2cc") });
        }
    }
}
