using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkingTails.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePetTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pet_AspNetUsers_OwnerId",
                table: "Pet");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Pet",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Pet_OwnerId",
                table: "Pet",
                newName: "IX_Pet_OrganizationId");

            migrationBuilder.AlterColumn<string>(
                name: "Age",
                table: "Pet",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Pet_AspNetUsers_OrganizationId",
                table: "Pet",
                column: "OrganizationId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pet_AspNetUsers_OrganizationId",
                table: "Pet");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "Pet",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Pet_OrganizationId",
                table: "Pet",
                newName: "IX_Pet_OwnerId");

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "Pet",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Pet_AspNetUsers_OwnerId",
                table: "Pet",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
