using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfficiencyTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class neededTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalNeededMinutes",
                table: "DailyEfficiencies",
                type: "decimal(10,4)",
                nullable: false,
                defaultValue: 0m,
                comment: "Total needed time .");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalNeededMinutes",
                table: "DailyEfficiencies");
        }
    }
}
