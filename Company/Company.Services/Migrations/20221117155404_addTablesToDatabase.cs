using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Company.Services.Migrations
{
    public partial class addTablesToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    departmentNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    departmentName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    departmentLocation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.departmentNo);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    loginNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    loginUserName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    loginPassword = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.loginNo);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    employeeNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employeeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Salary = table.Column<int>(type: "int", nullable: false),
                    lastModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EdepartmentNo = table.Column<int>(type: "int", nullable: false),
                    departmentNo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.employeeNo);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_departmentNo",
                        column: x => x.departmentNo,
                        principalTable: "Departments",
                        principalColumn: "departmentNo",
                        onDelete: ReferentialAction.Cascade); // very important thingy <-- ( if Department that was part of employee is deleted, so is employee deleted )
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_departmentNo",
                table: "Employees",
                column: "departmentNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
