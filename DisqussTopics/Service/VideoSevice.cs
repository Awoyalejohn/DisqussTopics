using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DisqussTopics.Helpers;
using Microsoft.Extensions.Options;

namespace DisqussTopics.Service
{
    public class VideoSevice : IVideoService
    {
        private readonly Cloudinary _cloudinary;

        public VideoSevice(IOptions<CloudinarySettings> configuration)
        {
            Account account = new(
                configuration.Value.CloudName,
                configuration.Value.ApiKey,
                configuration.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(account);

        }
        public async Task<VideoUploadResult> AddVideoAsync(IFormFile formFile)
        {
            var videoUploadResult = new VideoUploadResult();

            if (formFile != null)
            {
                using var stream = formFile.OpenReadStream();
                var uploadParams = new VideoUploadParams()
                {
                    File = new FileDescription(formFile.FileName, stream),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false,
                    EagerTransforms = new List<Transformation>()
                    {
                        new EagerTransformation().Width(300).Height(300).Crop("pad").AudioCodec("none"),
                        new EagerTransformation().Width(160).Height(100).Crop("crop").Gravity("south").AudioCodec("none"),
                    },
                    EagerAsync = true
                };
                videoUploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return videoUploadResult;
        }

        public async Task<DeletionResult> DeleteVideoAsync(string publicId)
        {
            var deletionparams = new DeletionParams(publicId);
            deletionparams.ResourceType = ResourceType.Video;
            var result = await _cloudinary.DestroyAsync(deletionparams);
            return result;
        }
    }
}
