using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TalkingTails.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationLinkRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donation_DonationPackage_DonationPackageId",
                table: "Donation");

            migrationBuilder.DropIndex(
                name: "IX_Donation_DonationPackageId",
                table: "Donation");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Donation");

            migrationBuilder.RenameColumn(
                name: "TransactionStatus",
                table: "Donation",
                newName: "PackageName");

            migrationBuilder.RenameColumn(
                name: "DonationPackageId",
                table: "Donation",
                newName: "DonationLinkRequestId");

            migrationBuilder.CreateTable(
                name: "DonationLinkRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderCode = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ReturnUrl = table.Column<string>(type: "text", nullable: false),
                    CancelUrl = table.Column<string>(type: "text", nullable: false),
                    CheckoutUrl = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    OrganizationId = table.Column<string>(type: "text", nullable: false),
                    PackageName = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationLinkRequest", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Donation_DonationLinkRequestId",
                table: "Donation",
                column: "DonationLinkRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Donation_DonationLinkRequest_DonationLinkRequestId",
                table: "Donation",
                column: "DonationLinkRequestId",
                principalTable: "DonationLinkRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donation_DonationLinkRequest_DonationLinkRequestId",
                table: "Donation");

            migrationBuilder.DropTable(
                name: "DonationLinkRequest");

            migrationBuilder.DropIndex(
                name: "IX_Donation_DonationLinkRequestId",
                table: "Donation");

            migrationBuilder.RenameColumn(
                name: "PackageName",
                table: "Donation",
                newName: "TransactionStatus");

            migrationBuilder.RenameColumn(
                name: "DonationLinkRequestId",
                table: "Donation",
                newName: "DonationPackageId");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Donation",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Donation_DonationPackageId",
                table: "Donation",
                column: "DonationPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Donation_DonationPackage_DonationPackageId",
                table: "Donation",
                column: "DonationPackageId",
                principalTable: "DonationPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
