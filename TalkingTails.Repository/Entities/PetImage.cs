namespace TalkingTails.Repository.Entities
{
    public class PetImage
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public Pet Pet { get; set; }
    }
}