using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Setting;

namespace TalkingTails.Business.Services
{
    public class FileService(BlobServiceClient blobServiceClient, IOptions<AzureStorageSettings> azureOptions)
        : IFileService
    {
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB in bytes

        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null.", nameof(file));
            }

            // Check if the file is too large
            if (file.Length > MaxFileSize)
            {
                var maxSizeInMb = MaxFileSize / (1024.0 * 1024.0);
                throw new ArgumentException($"File size exceeds {maxSizeInMb:F0}MB limit.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var contentType = GetContentType(extension);
            var containerClient = blobServiceClient.GetBlobContainerClient(azureOptions.Value.ContainerName);

            // Ensure the container exists
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var newFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobClient = containerClient.GetBlobClient(newFileName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(
                    stream,
                    new BlobUploadOptions { HttpHeaders = blobHttpHeaders }
                );
            }

            return blobClient.Uri.ToString();
        }

        public async Task<bool> DeleteAsync(string imageUri)
        {
            if (string.IsNullOrEmpty(imageUri))
            {
                return false;
            }

            var uri = new Uri(imageUri);
            var fileName = Path.GetFileName(uri.LocalPath);
            var containerClient = blobServiceClient.GetBlobContainerClient(azureOptions.Value.ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }

        private string GetContentType(string extension)
        {
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".doc" => "application/msword",
                ".docx" =>
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => throw new ArgumentException("Định dạng tập tin không được hỗ trợ.")
            };
        }
    }
}