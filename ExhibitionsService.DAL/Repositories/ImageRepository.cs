using ExhibitionsService.DAL.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ExhibitionsService.DAL.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly string rootPath;
        private readonly string storagePath;

        public ImageRepository(string _rootPath)
        {
            rootPath = _rootPath;
            storagePath = "paintings";
        }

        public async Task<string> SaveAsync(IFormFile image)
        {
            string path = Path.Combine(rootPath, storagePath);
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);

            string newImageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            path = Path.Combine(path, newImageName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return Path.Combine(storagePath, newImageName);
        }

        public void Delete(string imagePath)
        {
            string path = Path.Combine(rootPath, imagePath);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
