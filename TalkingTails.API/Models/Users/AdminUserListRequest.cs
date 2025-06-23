using System.ComponentModel.DataAnnotations;

namespace TalkingTails.API.Models.Users
{
    public class AdminUserListRequest
    {
        [Range(1, int.MaxValue)] public int? PageIndex { get; set; }
        [Range(1, 50)] public int? PageSize { get; set; }

        public string? SearchByName { get; set; }
        public string? SearchByEmail { get; set; }
        public string? Sort { get; set; }
    }
}