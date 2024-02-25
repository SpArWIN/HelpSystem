using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpSystem.Migrations
{
    /// <inheritdoc />
    public partial class StatmentInitialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("a87ac258-e63b-4743-ae0a-8f2c92e19221"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5feee312-08ac-42ac-ab1c-e53526f99a0d"));

            migrationBuilder.CreateTable(
                name: "Statements",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Statements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("cb6239d6-3559-4975-9bff-fd414520001d"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("129dccda-c54d-4aa1-bdc0-82c0b5dd8746"), null, null, null, null, null, new Guid("cb6239d6-3559-4975-9bff-fd414520001d") });

            migrationBuilder.CreateIndex(
                name: "IX_Statements_UserId",
                table: "Statements",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statements");

            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: new Guid("129dccda-c54d-4aa1-bdc0-82c0b5dd8746"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cb6239d6-3559-4975-9bff-fd414520001d"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Name", "Password", "RoleId" },
                values: new object[] { new Guid("5feee312-08ac-42ac-ab1c-e53526f99a0d"), "TotKtoVseZnaet", "Николай", "a60c1f75938be9607b94620c8925defe4d471cab0cab591fb418e89ff04b8ae7", 3 });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Age", "Description", "LastName", "Name", "Surname", "UserId" },
                values: new object[] { new Guid("a87ac258-e63b-4743-ae0a-8f2c92e19221"), null, null, null, null, null, new Guid("5feee312-08ac-42ac-ab1c-e53526f99a0d") });
        }
    }
}
