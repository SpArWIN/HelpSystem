using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpSystem.Migrations
{
    /// <inheritdoc />
    public partial class WarehouseService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("8c52b5ac-a1e2-4f57-8252-03089e5aeda9"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("595a3a23-cce8-4cc2-bc32-16e56e7a3f52"));

            migrationBuilder.AddColumn<bool>(
                name: "IsService",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "IsService",
                table: "Warehouses");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("595a3a23-cce8-4cc2-bc32-16e56e7a3f52"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("8c52b5ac-a1e2-4f57-8252-03089e5aeda9"), null, null, null, null, null, new Guid("595a3a23-cce8-4cc2-bc32-16e56e7a3f52") });
        }
    }
}
