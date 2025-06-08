using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkingTails.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFieldsFromAdoptedPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdoptionDate",
                table: "AdoptedPet");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AdoptedPet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AdoptionDate",
                table: "AdoptedPet",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AdoptedPet",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
