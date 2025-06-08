using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkingTails.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBlogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove search indexes of blog content
            migrationBuilder.Sql("DROP INDEX IF EXISTS public.trgm_idx_blog_content;");

            migrationBuilder.DropForeignKey(
                name: "FK_Blog_AspNetUsers_CreatorId",
                table: "Blog");

            migrationBuilder.DropIndex(
                name: "IX_Blog_CreatorId",
                table: "Blog");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Blog",
                newName: "ShortContent");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Blog",
                newName: "ContentUrl");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Blog",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Blog");

            migrationBuilder.RenameColumn(
                name: "ShortContent",
                table: "Blog",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "ContentUrl",
                table: "Blog",
                newName: "Content");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_CreatorId",
                table: "Blog",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_AspNetUsers_CreatorId",
                table: "Blog",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
