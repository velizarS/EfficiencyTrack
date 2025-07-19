using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfficiencyTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class changedDataStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_ApplicationUserId",
                table: "Employees");

            migrationBuilder.AddColumn<decimal>(
                name: "RequiredMinutes",
                table: "Entries",
                type: "decimal(10,4)",
                nullable: false,
                defaultValue: 0m,
                comment: "Theoretical required time for this operation.");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_IsHandled",
                table: "Feedbacks",
                column: "IsHandled");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ApplicationUserId",
                table: "Employees",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_IsHandled",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ApplicationUserId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RequiredMinutes",
                table: "Entries");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ApplicationUserId",
                table: "Employees",
                column: "ApplicationUserId");
        }
    }
}
