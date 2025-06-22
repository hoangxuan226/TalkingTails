namespace TalkingTails.Repository.Entities
{
    public class BlogComment
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string AuthorId { get; set; }
        public int BlogId { get; set; }
        public ApplicationUser Author { get; set; }
        public Blog Blog { get; set; }
    }
}