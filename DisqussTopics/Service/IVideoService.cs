using CloudinaryDotNet.Actions;

namespace DisqussTopics.Service
{
    public interface IVideoService
    {
        Task<VideoUploadResult> AddVideoAsync(IFormFile formFile);
        Task<DeletionResult> DeleteVideoAsync (string publicId);
    }
}
