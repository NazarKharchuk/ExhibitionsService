using Microsoft.AspNetCore.Http;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IImageRepository
    {
        Task<string> SaveAsync(IFormFile image);
        void Delete(string imagePath);
    }
}
