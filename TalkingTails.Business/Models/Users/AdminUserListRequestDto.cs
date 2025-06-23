namespace TalkingTails.Business.Models.Users
{
    public class AdminUserListRequestDto
    {
        public required int? PageIndex { get; set; }
        public required int? PageSize { get; set; }
        public string? SearchByName { get; set; }
        public string? SearchByEmail { get; set; }
        public string? Sort { get; set; }
    }
}