using HelloApi.Services.Interfaces;
using Microsoft.AspNetCore.StaticFiles;

namespace HelloApi.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        private string ImageFolderPath
        {
            get => $"{_environment.WebRootPath}/{_configuration.GetValue<string>("ImagesFolder")}/";
        }
        public FileService(
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<bool> DeleteImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            return await Task.Run(() =>
            {
                var path = ImageFolderPath + fileName;
                File.Delete(path);
                return !File.Exists(path);
            });
        }

        public bool IsImage(IFormFile image)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(image.FileName, out var contentType);
            return contentType.StartsWith("image/");
        }

        public async Task<string?> ReplaceImage(IFormFile image, string oldImgeName)
        {
            var newImage = await SaveImage(image);

            if (newImage is null)
                return oldImgeName;

            DeleteImage(oldImgeName);
            return newImage;
        }

        public async Task<string?> SaveImage(IFormFile image)
        {
            if (!IsImage(image)) return null;

            var newName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var path = ImageFolderPath + newName;

            using (var fs = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(fs);
            }

            return File.Exists(path) ? newName : null;
        }
    }
}
