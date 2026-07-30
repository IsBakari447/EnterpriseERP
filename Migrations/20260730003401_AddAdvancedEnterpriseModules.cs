using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseERP.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedEnterpriseModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankReconciliations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BankAccount = table.Column<string>(type: "TEXT", nullable: false),
                    StatementDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StatementBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ErpBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankReconciliations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashflowForecasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Period = table.Column<string>(type: "TEXT", nullable: false),
                    ExpectedInflow = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpectedOutflow = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Scenario = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashflowForecasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EcommerceConnections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Platform = table.Column<string>(type: "TEXT", nullable: false),
                    StoreName = table.Column<string>(type: "TEXT", nullable: false),
                    StoreUrl = table.Column<string>(type: "TEXT", nullable: false),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: false),
                    SyncProducts = table.Column<bool>(type: "INTEGER", nullable: false),
                    SyncOrders = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastSyncAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcommerceConnections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HrDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", nullable: false),
                    FileUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrDocuments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HrSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartTime = table.Column<string>(type: "TEXT", nullable: false),
                    EndTime = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrSchedules_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollSlips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Period = table.Column<string>(type: "TEXT", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollSlips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollSlips_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectBoards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ClientName = table.Column<string>(type: "TEXT", nullable: false),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Progress = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectBoards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTaskItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectBoardId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedTo = table.Column<string>(type: "TEXT", nullable: false),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Progress = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTaskItems_ProjectBoards_ProjectBoardId",
                        column: x => x.ProjectBoardId,
                        principalTable: "ProjectBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankReconciliations_Status_StatementDate",
                table: "BankReconciliations",
                columns: new[] { "Status", "StatementDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CashflowForecasts_Period_Scenario",
                table: "CashflowForecasts",
                columns: new[] { "Period", "Scenario" });

            migrationBuilder.CreateIndex(
                name: "IX_EcommerceConnections_Platform",
                table: "EcommerceConnections",
                column: "Platform");

            migrationBuilder.CreateIndex(
                name: "IX_HrDocuments_EmployeeId_DocumentType",
                table: "HrDocuments",
                columns: new[] { "EmployeeId", "DocumentType" });

            migrationBuilder.CreateIndex(
                name: "IX_HrSchedules_EmployeeId_WorkDate",
                table: "HrSchedules",
                columns: new[] { "EmployeeId", "WorkDate" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_EmployeeId_Status_StartDate",
                table: "LeaveRequests",
                columns: new[] { "EmployeeId", "Status", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollSlips_EmployeeId_Period",
                table: "PayrollSlips",
                columns: new[] { "EmployeeId", "Period" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBoards_Status_Deadline",
                table: "ProjectBoards",
                columns: new[] { "Status", "Deadline" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskItems_ProjectBoardId",
                table: "ProjectTaskItems",
                column: "ProjectBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskItems_Status_Deadline",
                table: "ProjectTaskItems",
                columns: new[] { "Status", "Deadline" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankReconciliations");

            migrationBuilder.DropTable(
                name: "CashflowForecasts");

            migrationBuilder.DropTable(
                name: "EcommerceConnections");

            migrationBuilder.DropTable(
                name: "HrDocuments");

            migrationBuilder.DropTable(
                name: "HrSchedules");

            migrationBuilder.DropTable(
                name: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "PayrollSlips");

            migrationBuilder.DropTable(
                name: "ProjectTaskItems");

            migrationBuilder.DropTable(
                name: "ProjectBoards");
        }
    }
}
