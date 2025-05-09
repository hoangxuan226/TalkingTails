using Microsoft.AspNetCore.Http;

namespace TalkingTails.Business.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file);
        Task<bool> DeleteAsync(string imageUri);
    }
}