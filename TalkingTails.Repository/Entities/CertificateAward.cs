namespace TalkingTails.Repository.Entities
{
    public class CertificateAward
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserId { get; set; }
        public double TotalAmount { get; set; }
        public string CertificateUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser User { get; set; }
    }
}