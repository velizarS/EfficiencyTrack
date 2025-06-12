using System;
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
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_ApplicationUserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_ApplicationUserId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees_LeaderId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AspNetUsers_UserId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Employees_LeaderId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserLogins_ApplicationUserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "LeaderId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "TotalNeededMinutes",
                table: "DailyEfficiencies");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserLogins");

            migrationBuilder.AlterTable(
                name: "DailyEfficiencies",
                comment: "Represents the daily efficiency of an employee, calculated automatically based on performed tasks and shift time.",
                oldComment: "Represents the daily efficiency of an employee, calculated based on the tasks performed and time worked.");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "Feedbacks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "The full name of the employee providing the feedback.");

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftManagerUserId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Reference to the employee's shift manager (ApplicationUser).");

            migrationBuilder.AlterColumn<decimal>(
                name: "EfficiencyPercentage",
                table: "DailyEfficiencies",
                type: "decimal(10,4)",
                nullable: false,
                comment: "Calculated efficiency as (total needed minutes based on operations / shift time) * 100.",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,4)",
                oldComment: "Calculated efficiency as (TotalNeededMinutes / TotalWorkedMinutes) * 100.");

            migrationBuilder.AddColumn<DateTime>(
                name: "ComputedOn",
                table: "DailyEfficiencies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Timestamp when the efficiency was last calculated.");

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftId",
                table: "DailyEfficiencies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "The shift during which the employee worked.");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ShiftManagerUserId",
                table: "Employees",
                column: "ShiftManagerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyEfficiencies_ShiftId",
                table: "DailyEfficiencies",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyEfficiencies_Shifts_ShiftId",
                table: "DailyEfficiencies",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_ApplicationUserId",
                table: "Employees",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_ShiftManagerUserId",
                table: "Employees",
                column: "ShiftManagerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyEfficiencies_Shifts_ShiftId",
                table: "DailyEfficiencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_ApplicationUserId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_ShiftManagerUserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ShiftManagerUserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_DailyEfficiencies_ShiftId",
                table: "DailyEfficiencies");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "ShiftManagerUserId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ComputedOn",
                table: "DailyEfficiencies");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "DailyEfficiencies");

            migrationBuilder.AlterTable(
                name: "DailyEfficiencies",
                comment: "Represents the daily efficiency of an employee, calculated based on the tasks performed and time worked.",
                oldComment: "Represents the daily efficiency of an employee, calculated automatically based on performed tasks and shift time.");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "Feedbacks",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "The unique code for the employee.");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Feedbacks",
                type: "uniqueidentifier",
                nullable: true,
                comment: "The user who provided the feedback.");

            migrationBuilder.AddColumn<Guid>(
                name: "LeaderId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Reference to the employee's leader.");

            migrationBuilder.AlterColumn<decimal>(
                name: "EfficiencyPercentage",
                table: "DailyEfficiencies",
                type: "decimal(10,4)",
                nullable: false,
                comment: "Calculated efficiency as (TotalNeededMinutes / TotalWorkedMinutes) * 100.",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,4)",
                oldComment: "Calculated efficiency as (total needed minutes based on operations / shift time) * 100.");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalNeededMinutes",
                table: "DailyEfficiencies",
                type: "decimal(10,4)",
                nullable: false,
                defaultValue: 0m,
                comment: "Sum of the theoretical time required for all tasks performed that day.");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "AspNetUserRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "AspNetUserLogins",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LeaderId",
                table: "Employees",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_ApplicationUserId",
                table: "AspNetUserLogins",
                column: "ApplicationUserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_ApplicationUserId",
                table: "Employees",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees_LeaderId",
                table: "Employees",
                column: "LeaderId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AspNetUsers_UserId",
                table: "Feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
