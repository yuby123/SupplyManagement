using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class createDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_m_company",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    address = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    phone_number = table.Column<string>(type: "nvarchar(15)", nullable: false),
                    foto = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_m_company", x => x.guid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_m_roles",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_m_roles", x => x.guid);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_m_vendor",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    bidang_usaha = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    jenis_perusahaan = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    status_vendor = table.Column<int>(type: "int", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_m_vendor", x => x.guid);
                    table.ForeignKey(
                        name: "FK_tb_m_vendor_tb_m_company_guid",
                        column: x => x.guid,
                        principalTable: "tb_m_company",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tb_m_accounts",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    otp = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    is_used = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    expired_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    role_guid = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_m_accounts", x => x.guid);
                    table.ForeignKey(
                        name: "FK_tb_m_accounts_tb_m_company_guid",
                        column: x => x.guid,
                        principalTable: "tb_m_company",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_m_accounts_tb_m_roles_role_guid",
                        column: x => x.role_guid,
                        principalTable: "tb_m_roles",
                        principalColumn: "guid",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "tb_m_company",
                columns: new[] { "guid", "address", "created_date", "email", "foto", "modified_date", "name", "phone_number" },
                values: new object[,]
                {
                    { new Guid("4e341bd3-69ed-4509-affb-6a738cae37f0"), "null", new DateTime(2023, 12, 30, 9, 9, 21, 500, DateTimeKind.Local).AddTicks(2457), "manager123@mail.com", "nul", null, "Manager Logistic", "11111" },
                    { new Guid("5cdbb962-a21e-4aba-8fae-4227a37b0878"), "null", new DateTime(2023, 12, 30, 9, 9, 21, 500, DateTimeKind.Local).AddTicks(2424), "admin2023@mail.com", "null", null, "Admin", "00000" }
                });

            migrationBuilder.InsertData(
                table: "tb_m_roles",
                columns: new[] { "guid", "created_date", "modified_date", "name" },
                values: new object[,]
                {
                    { new Guid("4ef36046-f48b-40b4-add6-bb1fea6a2490"), null, null, "admin" },
                    { new Guid("686d0669-9df9-469e-9f5c-b31d10d43f1c"), null, null, "vendor" },
                    { new Guid("aa422e40-1310-486e-950b-a8ba98eceedb"), null, null, "manager" }
                });

            migrationBuilder.InsertData(
                table: "tb_m_accounts",
                columns: new[] { "guid", "created_date", "expired_time", "is_used", "modified_date", "otp", "password", "role_guid", "status" },
                values: new object[] { new Guid("4e341bd3-69ed-4509-affb-6a738cae37f0"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, 0, "$2a$12$pOhiKpjnuqdOn7wTjWccFu6zVVti4cp86HWpogMv0GMH3UtYq/hsG", new Guid("aa422e40-1310-486e-950b-a8ba98eceedb"), 1 });

            migrationBuilder.InsertData(
                table: "tb_m_accounts",
                columns: new[] { "guid", "created_date", "expired_time", "is_used", "modified_date", "otp", "password", "role_guid", "status" },
                values: new object[] { new Guid("5cdbb962-a21e-4aba-8fae-4227a37b0878"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, 0, "$2a$12$ZdWT4iagxSXhUwHFpMwwi..PiiamL2V3AzMd4sDWbVoMa0jJYOyMq", new Guid("4ef36046-f48b-40b4-add6-bb1fea6a2490"), 1 });

            migrationBuilder.CreateIndex(
                name: "IX_tb_m_accounts_role_guid",
                table: "tb_m_accounts",
                column: "role_guid");

            migrationBuilder.CreateIndex(
                name: "IX_tb_m_company_email",
                table: "tb_m_company",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tb_m_company_phone_number",
                table: "tb_m_company",
                column: "phone_number",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_m_accounts");

            migrationBuilder.DropTable(
                name: "tb_m_vendor");

            migrationBuilder.DropTable(
                name: "tb_m_roles");

            migrationBuilder.DropTable(
                name: "tb_m_company");
        }
    }
}
