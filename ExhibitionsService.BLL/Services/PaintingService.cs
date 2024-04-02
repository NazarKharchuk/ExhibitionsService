using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace ExhibitionsService.BLL.Services
{
    public class PaintingService : IPaintingService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public PaintingService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(PaintingDTO entity, IFormFile image)
        {
            ValidateEntity(entity);

            if (await uow.Painters.GetByIdAsync(entity.PainterId) == null)
                throw new ValidationException(entity.GetType().Name, nameof(entity.PainterId), "Художника з вказаним Id не існує");

            ValidateImage(image);

            string imagePath = await uow.Images.SaveAsync(image);

            entity.PaintingId = 0;
            entity.ImagePath = imagePath;
            await uow.Paintings.CreateAsync(mapper.Map<Painting>(entity));
            await uow.SaveAsync();
        }

        public async Task<PaintingDTO> UpdateAsync(PaintingDTO entity, IFormFile? image)
        {
            var existingEntity = await CheckEntityPresence(entity.PaintingId);

            ValidateEntity(entity);
            
            if(image != null)
            {
                ValidateImage(image);

                string newImagePath = await uow.Images.SaveAsync(image);
                uow.Images.Delete(existingEntity.ImagePath);

                existingEntity.ImagePath = newImagePath;
            }

            existingEntity.Name = entity.Name;
            existingEntity.Description = entity.Description;
            existingEntity.CretionDate = entity.CretionDate;
            existingEntity.Width = entity.Width;
            existingEntity.Height = entity.Height;
            existingEntity.Location = entity.Location;

            await uow.Paintings.UpdateAsync(existingEntity);
            await uow.SaveAsync();

            return mapper.Map<PaintingDTO>(existingEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            uow.Images.Delete(existingEntity.ImagePath);

            await uow.Paintings.DeleteAsync(id);
            await uow.SaveAsync();
        }

        public async Task<PaintingDTO?> GetByIdAsync(int id)
        {
            var existingEntity = await CheckEntityPresence(id);

            return mapper.Map<PaintingDTO>(existingEntity);
        }

        public async Task<List<PaintingDTO>> GetAllAsync()
        {
            return mapper.Map<List<PaintingDTO>>((await uow.Paintings.GetAllAsync()).ToList());
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(PaintingDTO entity)
        {
            if (entity.Name.IsNullOrEmpty() || entity.Description.Length > 50)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Name));

            if (entity.Description.IsNullOrEmpty() || entity.Description.Length > 500)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Description));

            if (entity.CretionDate > DateTime.Now)
                throw new ValidationException(entity.GetType().Name, nameof(entity.CretionDate));

            if (entity.Width <= 0)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Width));

            if (entity.Height <= 0)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Height));

            if (!entity.Location.IsNullOrEmpty() && entity.Location.Length > 100)
                throw new ValidationException(entity.GetType().Name, nameof(entity.Location));
        }

        private void ValidateImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ValidationException("PaintingDTO", "Image", "Файл зображення не був завантажений.");

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (extension.IsNullOrEmpty() || !allowedExtensions.Contains(extension))
                throw new ValidationException("PaintingDTO", "Image", "Дозволені формати зображення: jpg, jpeg, png, gif.");

            const int maxFileSize = 50 * 1024 * 1024; // 50 MB
            if (image.Length > maxFileSize)
                throw new ValidationException("PaintingDTO", "Image", "Розмір файлу перевищує допустимий ліміт (50 MB).");
        }

        private async Task<Painting?> CheckEntityPresence(int id)
        {
            Painting? existingEntity = await uow.Paintings.GetByIdAsync(id);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(PaintingDTO).Name, id);

            return existingEntity;
        }
    }
}
