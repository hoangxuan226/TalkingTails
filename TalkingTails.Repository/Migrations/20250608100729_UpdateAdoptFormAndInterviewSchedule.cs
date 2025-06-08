using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkingTails.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdoptFormAndInterviewSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSchedule_AspNetUsers_IntervieweeId",
                table: "InterviewSchedule");

            migrationBuilder.DropIndex(
                name: "IX_InterviewSchedule_IntervieweeId",
                table: "InterviewSchedule");

            migrationBuilder.DropColumn(
                name: "IntervieweeId",
                table: "InterviewSchedule");

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "AdoptionForm",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "AdoptionForm");

            migrationBuilder.AddColumn<string>(
                name: "IntervieweeId",
                table: "InterviewSchedule",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_IntervieweeId",
                table: "InterviewSchedule",
                column: "IntervieweeId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSchedule_AspNetUsers_IntervieweeId",
                table: "InterviewSchedule",
                column: "IntervieweeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
