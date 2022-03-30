using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace POS.DataAccessLayer.Migrations
{
    public partial class companyMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 160, nullable: false),
                    Address = table.Column<string>(maxLength: 500, nullable: true),
                    FaxNo = table.Column<int>(nullable: false),
                    ContactNo = table.Column<int>(nullable: false),
                    Email = table.Column<string>(maxLength: 60, nullable: true),
                    CreateBy = table.Column<string>(maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    CreateAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
