using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TalkingTails.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddUnAccentExtensionAndSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Set schema for all operations
            migrationBuilder.Sql("SET search_path TO public;");

            // Enable extensions for search in public schema
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS unaccent SCHEMA public;");
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm SCHEMA public;");

            // Verify unaccent extension exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_extension WHERE extname = 'unaccent' AND extnamespace = (SELECT oid FROM pg_namespace WHERE nspname = 'public')) THEN
                        RAISE EXCEPTION 'unaccent extension is not installed in public schema';
                    END IF;
                END $$;
            ");

            // Create normalize function in public schema
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.normalize_vietnamese(text) RETURNS text AS $$
                SELECT lower(public.unaccent(replace(replace($1, 'đ', 'd'), 'Đ', 'D')));
                $$ LANGUAGE SQL IMMUTABLE;
            ");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Organization_Description",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Organization_MeetLink",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoverImageUrl = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Species = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blog_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateAward",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TotalAmount = table.Column<double>(type: "double precision", nullable: false),
                    CertificateUrl = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateAward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateAward_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonationPackage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationPackage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    RedirectUrl = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PetName = table.Column<string>(type: "text", nullable: false),
                    Species = table.Column<string>(type: "text", nullable: false),
                    Breed = table.Column<string>(type: "text", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    LivingEnvironmentNeeds = table.Column<string[]>(type: "jsonb", nullable: false),
                    Information = table.Column<string>(type: "jsonb", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pet_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Donation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    DonationPackageId = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    TransactionStatus = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donation_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Donation_DonationPackage_DonationPackageId",
                        column: x => x.DonationPackageId,
                        principalTable: "DonationPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdoptionForm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormSubmitterId = table.Column<string>(type: "text", nullable: false),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    ContactPhone = table.Column<string>(type: "text", nullable: false),
                    ContactEmail = table.Column<string>(type: "text", nullable: false),
                    ContactAddress = table.Column<string>(type: "text", nullable: false),
                    LivingConditions = table.Column<string>(type: "text", nullable: false),
                    HasOtherPets = table.Column<bool>(type: "boolean", nullable: false),
                    ReasonToAdopt = table.Column<string>(type: "text", nullable: false),
                    AvailableContactTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionForm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdoptionForm_AspNetUsers_FormSubmitterId",
                        column: x => x.FormSubmitterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionForm_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PetImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetImage_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdoptedPet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    AdopterId = table.Column<string>(type: "text", nullable: false),
                    AdoptionFormId = table.Column<int>(type: "integer", nullable: false),
                    AdoptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptedPet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdoptedPet_AdoptionForm_AdoptionFormId",
                        column: x => x.AdoptionFormId,
                        principalTable: "AdoptionForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptedPet_AspNetUsers_AdopterId",
                        column: x => x.AdopterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptedPet_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdoptionFormId = table.Column<int>(type: "integer", nullable: false),
                    IntervieweeId = table.Column<string>(type: "text", nullable: false),
                    InterviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewSchedule_AdoptionForm_AdoptionFormId",
                        column: x => x.AdoptionFormId,
                        principalTable: "AdoptionForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewSchedule_AspNetUsers_IntervieweeId",
                        column: x => x.IntervieweeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptedPet_AdopterId",
                table: "AdoptedPet",
                column: "AdopterId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptedPet_AdoptionFormId",
                table: "AdoptedPet",
                column: "AdoptionFormId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptedPet_PetId",
                table: "AdoptedPet",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionForm_FormSubmitterId",
                table: "AdoptionForm",
                column: "FormSubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionForm_PetId",
                table: "AdoptionForm",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_CreatorId",
                table: "Blog",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Slug",
                table: "Blog",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificateAward_UserId",
                table: "CertificateAward",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Donation_DonationPackageId",
                table: "Donation",
                column: "DonationPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Donation_UserId",
                table: "Donation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_AdoptionFormId",
                table: "InterviewSchedule",
                column: "AdoptionFormId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSchedule_IntervieweeId",
                table: "InterviewSchedule",
                column: "IntervieweeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_OwnerId",
                table: "Pet",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_Slug",
                table: "Pet",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PetImage_PetId",
                table: "PetImage",
                column: "PetId");

            // Search indexes for searchable fields in public schema
            // For Blog.Title
            migrationBuilder.Sql(@"
                CREATE INDEX trgm_idx_blog_title ON public.""Blog""
                USING GIN (public.normalize_vietnamese(""Title"") gin_trgm_ops);
            ");

            // For Blog.Content
            migrationBuilder.Sql(@"
                CREATE INDEX trgm_idx_blog_content ON public.""Blog""
                USING GIN (public.normalize_vietnamese(""Content"") gin_trgm_ops);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove search indexes
            migrationBuilder.Sql("DROP INDEX IF EXISTS public.trgm_idx_blog_title;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS public.trgm_idx_blog_content;");

            // Drop normalize_vietnamese function
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS public.normalize_vietnamese(text);");

            // Drop extensions (only if no other migrations depend on them)
            migrationBuilder.Sql("DROP EXTENSION IF EXISTS unaccent;");
            migrationBuilder.Sql("DROP EXTENSION IF EXISTS pg_trgm;");

            migrationBuilder.DropTable(
                name: "AdoptedPet");

            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "CertificateAward");

            migrationBuilder.DropTable(
                name: "Donation");

            migrationBuilder.DropTable(
                name: "InterviewSchedule");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PetImage");

            migrationBuilder.DropTable(
                name: "DonationPackage");

            migrationBuilder.DropTable(
                name: "AdoptionForm");

            migrationBuilder.DropTable(
                name: "Pet");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Organization_Description",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Organization_MeetLink",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AspNetUsers");
        }
    }
}
