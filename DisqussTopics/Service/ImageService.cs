using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DisqussTopics.Helpers;
using Microsoft.Extensions.Options;

namespace DisqussTopics.Service
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(IOptions<CloudinarySettings> configuration)
        {
            Account account = new(
                configuration.Value.CloudName,
                configuration.Value.ApiKey,
                configuration.Value.ApiSecret
                );
            _cloudinary = new Cloudinary( account );

        }

        public async Task<ImageUploadResult> AddImageAsync(IFormFile formFile)
        {
            var imageUploadResult = new ImageUploadResult();

            if (formFile != null )
            {
                using var stream = formFile.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                };
                imageUploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return imageUploadResult;
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}
