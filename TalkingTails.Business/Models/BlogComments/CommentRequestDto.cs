namespace TalkingTails.Business.Models.BlogComments
{
    public class CommentRequestDto
    {
        public int BlogId { get; set; }
        public required string Content { get; set; }
        public required string AuthorId { get; set; }
    }
}