using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfficiencyTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "AspNetUserRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "AspNetUserLogins",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "The name of the department."),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "When is created"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is created"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "When is modified"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "By who is modified"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Is deleted"),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is deleted")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                },
                comment: "Represents a department within the organization.");

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The name of the shift."),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false, comment: "The duration of the shift in total minutes."),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "When is created"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is created"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "When is modified"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "By who is modified"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Is deleted"),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is deleted")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                },
                comment: "Represents a work shift in the system.");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The unique code for the employee."),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The first name of the employee."),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "The middle name of the employee."),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The last name of the employee."),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, comment: "Indicates if the employee is currently active."),
                    LeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "Reference to the employee's leader."),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The department to which the employee belongs."),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "Reference to the associated application user."),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "When is created"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is created"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "When is modified"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "By who is modified"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Is deleted"),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is deleted")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                },
                comment: "Represents an employee in the system.");

            migrationBuilder.CreateTable(
                name: "Routings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The unique code for the routing operation."),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "The description of the routing operation."),
                    Zone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The zone where the routing operation is performed."),
                    MinutesPerPiece = table.Column<decimal>(type: "decimal(10,4)", nullable: false, comment: "The time in minutes required to process one piece in this routing operation."),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The department responsible for the routing operation."),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "When is created"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is created"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "When is modified"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "By who is modified"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Is deleted"),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is deleted")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routings_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Represents a routing operation in the system.");

            migrationBuilder.CreateTable(
                name: "DailyEfficiencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "The date for which this daily efficiency entry is calculated."),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The employee this daily efficiency record belongs to."),
                    TotalNeededMinutes = table.Column<decimal>(type: "decimal(10,4)", nullable: false, comment: "Sum of the theoretical time required for all tasks performed that day."),
                    TotalWorkedMinutes = table.Column<decimal>(type: "decimal(10,4)", nullable: false, comment: "Total actual time worked by the employee on that day."),
                    EfficiencyPercentage = table.Column<decimal>(type: "decimal(10,4)", nullable: false, comment: "Calculated efficiency as (TotalNeededMinutes / TotalWorkedMinutes) * 100."),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "When is created"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is created"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "When is modified"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "By who is modified"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Is deleted"),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is deleted")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyEfficiencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyEfficiencies_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Represents the daily efficiency of an employee, calculated based on the tasks performed and time worked.");

            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "The date when the entry was recorded."),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The employee associated with this entry."),
                    RoutingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The routing operation associated with this entry."),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The shift during which this entry was recorded."),
                    Pieces = table.Column<int>(type: "int", nullable: false, comment: "The number of pieces produced."),
                    Scrap = table.Column<int>(type: "int", nullable: false, comment: "The number of scrap pieces produced."),
                    WorkedMinutes = table.Column<decimal>(type: "decimal(10,4)", nullable: false, comment: "The number of minutes worked during the entry."),
                    EfficiencyForOperation = table.Column<decimal>(type: "decimal(10,4)", nullable: false, comment: "The efficiency percentage for the operation."),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "When is created"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is created"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "When is modified"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "By who is modified"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Is deleted"),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "By who is deleted")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_Routings_RoutingId",
                        column: x => x.RoutingId,
                        principalTable: "Routings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Represents a production entry for an employee, including details about the operation performed, pieces produced, and efficiency metrics.");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_ApplicationUserId",
                table: "AspNetUserLogins",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyEfficiencies_EmployeeId",
                table: "DailyEfficiencies",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyEfficiency_Date_Employee",
                table: "DailyEfficiencies",
                columns: new[] { "Date", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ApplicationUserId",
                table: "Employees",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Code",
                table: "Employees",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LeaderId",
                table: "Employees",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_Date_Employee_Routing_Shift",
                table: "Entries",
                columns: new[] { "Date", "EmployeeId", "RoutingId", "ShiftId", "Pieces", "WorkedMinutes" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entries_EmployeeId",
                table: "Entries",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_RoutingId",
                table: "Entries",
                column: "RoutingId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_ShiftId",
                table: "Entries",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Routings_Code",
                table: "Routings",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routings_DepartmentId",
                table: "Routings",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_Name",
                table: "Shifts",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_ApplicationUserId",
                table: "AspNetUserLogins",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_ApplicationUserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "DailyEfficiencies");

            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Routings");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserLogins_ApplicationUserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserLogins");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
