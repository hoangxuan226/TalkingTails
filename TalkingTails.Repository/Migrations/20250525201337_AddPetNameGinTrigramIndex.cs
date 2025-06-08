using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkingTails.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddPetNameGinTrigramIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Enable pg_trgm extension (if not already enabled)
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");

            // Create GIN index on normalized PetName
            migrationBuilder.Sql(@"
                CREATE INDEX trgm_idx_pet_name ON public.""Pet""
                USING GIN (public.normalize_vietnamese(""PetName"") gin_trgm_ops);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the index for rollback
            migrationBuilder.Sql("DROP INDEX IF EXISTS trgm_idx_pet_name;");
        }
    }
}
