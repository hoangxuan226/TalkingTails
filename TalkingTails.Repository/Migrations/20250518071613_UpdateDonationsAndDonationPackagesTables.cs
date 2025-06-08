using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkingTails.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDonationsAndDonationPackagesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "DonationPackage",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Donation",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Donation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "TotalAmount",
                table: "CertificateAward",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateIndex(
                name: "IX_Donation_OrganizationId",
                table: "Donation",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Donation_AspNetUsers_OrganizationId",
                table: "Donation",
                column: "OrganizationId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donation_AspNetUsers_OrganizationId",
                table: "Donation");

            migrationBuilder.DropIndex(
                name: "IX_Donation_OrganizationId",
                table: "Donation");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Donation");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "DonationPackage",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Donation",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "TotalAmount",
                table: "CertificateAward",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
