using CloudinaryDotNet.Actions;

namespace DisqussTopics.Service
{
    public interface IImageService
    {
        Task<ImageUploadResult> AddImageAsync(IFormFile formFile);
        Task<DeletionResult> DeleteImageAsync(string publicId);
    }
}
