namespace TalkingTails.Business.Models.BlogComments
{
    public class BlogCommentListRequestDto
    {
        public required int BlogId { get; set; }
        public required int? PageIndex { get; set; }
        public required int? PageSize { get; set; }
    }
}