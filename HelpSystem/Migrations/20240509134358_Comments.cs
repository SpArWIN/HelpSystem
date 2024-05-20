using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpSystem.Migrations
{
    /// <inheritdoc />
    public partial class Comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "CommentDebbiting",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CommentDebbiting",
                table: "Products");

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
    }
}
