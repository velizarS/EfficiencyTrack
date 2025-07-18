using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfficiencyTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Shifts",
                type: "nvarchar(max)",
                nullable: true,
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "By who is deleted");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Routings",
                type: "nvarchar(max)",
                nullable: true,
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "By who is deleted");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Entries",
                type: "nvarchar(max)",
                nullable: true,
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "By who is deleted");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "By who is deleted");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true,
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "By who is deleted");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "DailyEfficiencies",
                type: "nvarchar(max)",
                nullable: true,
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "By who is deleted");

            _ = migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "The user who provided the feedback."),
                    EmployeeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "The unique code for the employee."),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false, comment: "The feedback message provided by the user."),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "The date and time when the feedback was created."),
                    IsHandled = table.Column<bool>(type: "bit", nullable: false, comment: "Indicates whether the feedback has been handled."),
                    HandledAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "The date and time when the feedback was handled.")
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_Feedbacks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                },
                comment: "Represents user feedback in the system.");

            _ = migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "Feedbacks");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Shifts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "By who is deleted");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Routings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "By who is deleted");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Entries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "By who is deleted");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "By who is deleted");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "By who is deleted");

            _ = migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "DailyEfficiencies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "By who is deleted",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "By who is deleted");
        }
    }
}
