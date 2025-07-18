using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfficiencyTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_ApplicationUserId",
                table: "AspNetUserLogins");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_ApplicationUserId",
                table: "Employees");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees_LeaderId",
                table: "Employees");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AspNetUsers_UserId",
                table: "Feedbacks");

            _ = migrationBuilder.DropIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks");

            _ = migrationBuilder.DropIndex(
                name: "IX_Employees_LeaderId",
                table: "Employees");

            _ = migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles");

            _ = migrationBuilder.DropIndex(
                name: "IX_AspNetUserLogins_ApplicationUserId",
                table: "AspNetUserLogins");

            _ = migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "Feedbacks");

            _ = migrationBuilder.DropColumn(
                name: "UserId",
                table: "Feedbacks");

            _ = migrationBuilder.DropColumn(
                name: "LeaderId",
                table: "Employees");

            _ = migrationBuilder.DropColumn(
                name: "TotalNeededMinutes",
                table: "DailyEfficiencies");

            _ = migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserRoles");

            _ = migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserLogins");

            _ = migrationBuilder.AlterTable(
                name: "DailyEfficiencies",
                comment: "Represents the daily efficiency of an employee, calculated automatically based on performed tasks and shift time.",
                oldComment: "Represents the daily efficiency of an employee, calculated based on the tasks performed and time worked.");

            _ = migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "Feedbacks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "The full name of the employee providing the feedback.");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "ShiftManagerUserId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Reference to the employee's shift manager (ApplicationUser).");

            _ = migrationBuilder.AlterColumn<decimal>(
                name: "EfficiencyPercentage",
                table: "DailyEfficiencies",
                type: "decimal(10,4)",
                nullable: false,
                comment: "Calculated efficiency as (total needed minutes based on operations / shift time) * 100.",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,4)",
                oldComment: "Calculated efficiency as (TotalNeededMinutes / TotalWorkedMinutes) * 100.");

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "ComputedOn",
                table: "DailyEfficiencies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Timestamp when the efficiency was last calculated.");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "ShiftId",
                table: "DailyEfficiencies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "The shift during which the employee worked.");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Employees_ShiftManagerUserId",
                table: "Employees",
                column: "ShiftManagerUserId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_DailyEfficiencies_ShiftId",
                table: "DailyEfficiencies",
                column: "ShiftId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_DailyEfficiencies_Shifts_ShiftId",
                table: "DailyEfficiencies",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_ApplicationUserId",
                table: "Employees",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_ShiftManagerUserId",
                table: "Employees",
                column: "ShiftManagerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropForeignKey(
                name: "FK_DailyEfficiencies_Shifts_ShiftId",
                table: "DailyEfficiencies");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_ApplicationUserId",
                table: "Employees");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_ShiftManagerUserId",
                table: "Employees");

            _ = migrationBuilder.DropIndex(
                name: "IX_Employees_ShiftManagerUserId",
                table: "Employees");

            _ = migrationBuilder.DropIndex(
                name: "IX_DailyEfficiencies_ShiftId",
                table: "DailyEfficiencies");

            _ = migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "Feedbacks");

            _ = migrationBuilder.DropColumn(
                name: "ShiftManagerUserId",
                table: "Employees");

            _ = migrationBuilder.DropColumn(
                name: "ComputedOn",
                table: "DailyEfficiencies");

            _ = migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "DailyEfficiencies");

            _ = migrationBuilder.AlterTable(
                name: "DailyEfficiencies",
                comment: "Represents the daily efficiency of an employee, calculated based on the tasks performed and time worked.",
                oldComment: "Represents the daily efficiency of an employee, calculated automatically based on performed tasks and shift time.");

            _ = migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "Feedbacks",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "The unique code for the employee.");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Feedbacks",
                type: "uniqueidentifier",
                nullable: true,
                comment: "The user who provided the feedback.");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "LeaderId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Reference to the employee's leader.");

            _ = migrationBuilder.AlterColumn<decimal>(
                name: "EfficiencyPercentage",
                table: "DailyEfficiencies",
                type: "decimal(10,4)",
                nullable: false,
                comment: "Calculated efficiency as (TotalNeededMinutes / TotalWorkedMinutes) * 100.",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,4)",
                oldComment: "Calculated efficiency as (total needed minutes based on operations / shift time) * 100.");

            _ = migrationBuilder.AddColumn<decimal>(
                name: "TotalNeededMinutes",
                table: "DailyEfficiencies",
                type: "decimal(10,4)",
                nullable: false,
                defaultValue: 0m,
                comment: "Sum of the theoretical time required for all tasks performed that day.");

            _ = migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "AspNetUserRoles",
                type: "uniqueidentifier",
                nullable: true);

            _ = migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "AspNetUserLogins",
                type: "uniqueidentifier",
                nullable: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Employees_LeaderId",
                table: "Employees",
                column: "LeaderId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_ApplicationUserId",
                table: "AspNetUserLogins",
                column: "ApplicationUserId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_ApplicationUserId",
                table: "AspNetUserLogins",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_ApplicationUserId",
                table: "Employees",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees_LeaderId",
                table: "Employees",
                column: "LeaderId",
                principalTable: "Employees",
                principalColumn: "Id");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AspNetUsers_UserId",
                table: "Feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
