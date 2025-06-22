using System.Linq.Expressions;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.BlogComments
{
    public class BlogCommentDto : IMappable<BlogComment>
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string AuthorName { get; set; }
        public required string AuthorAvatarUrl { get; set; }

        public static Dictionary<string, Expression<Func<BlogComment, object>>> Mappings { get; } = new()
        {
            {
                nameof(AuthorName), bc => bc.Author.Name ?? bc.Author.UserName ?? bc.Author.Email ?? string.Empty
            },
            {
                nameof(AuthorAvatarUrl), bc => bc.Author.ProfileImage ?? string.Empty
            }
        };
    }
}