using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfficiencyTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmployeModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if the employee is currently active.");
        }
    }
}
