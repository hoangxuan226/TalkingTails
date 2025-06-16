using Microsoft.AspNetCore.Identity;

namespace TalkingTails.Repository.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ProfileImage { get; set; }
        public OrganizationDetails? Organization { get; set; }
        public CustomerDetails? Customer { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = [];
        public ICollection<Pet> Pets { get; set; }
        public ICollection<AdoptionForm> AdoptionForms { get; set; }
        public ICollection<Donation> Donations { get; set; } // Donations made by the customer
        public ICollection<Donation> DonationsReceived { get; set; } // Donations received by the organization
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<CertificateAward> CertificateAwards { get; set; }
        public ICollection<AdoptedPet> AdoptedPets { get; set; }
    }
}