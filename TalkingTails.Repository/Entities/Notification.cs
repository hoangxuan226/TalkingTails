namespace TalkingTails.Repository.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser User { get; set; }
    }
}