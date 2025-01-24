using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LearnerDuo.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LearnerDuo.Services
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }

    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IConfiguration config)
        {

            var acc = new Account
            {
                Cloud = config["CloudinarySettings:CloudName"],
                ApiKey = config["CloudinarySettings:ApiKey"],
                ApiSecret = config["CloudinarySettings:ApiSecret"],

            };

            // sing in to get information of account to add or delete photo in cloudinary
            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}
