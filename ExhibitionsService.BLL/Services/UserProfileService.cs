using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Enums;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace ExhibitionsService.BLL.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork uow;
        private readonly UserManager<User> userManager;

        public UserProfileService(IUnitOfWork _uow, UserManager<User> _userManager)
        {
            uow = _uow;
            userManager = _userManager;
        }

        public async Task CreateAsync(UserProfileDTO entity)
        {
            ValidateEntity(entity);
            ValidateEmail(entity.Email);

            var newUser = new User
            {
                UserName = entity.Email,
                Email = entity.Email,
                UserProfile = new UserProfile
                {
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    JoiningDate = DateTime.UtcNow
                }
            };

            var createResult = await userManager.CreateAsync(newUser, entity.Password);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, Role.Viewer.ToString());
            }
            else
            {
                var errorMessages = string.Join("\n", createResult.Errors.Select(e => e.Description));
                throw new Exception("Помилка під час додавання користувача: \n" + errorMessages);
            }
        }

        public async Task<UserProfileDTO> UpdateAsync(UserProfileDTO entity)
        {
            ValidateEntity(entity);

            var existingEntities = await CheckEntityPresence(entity.ProfileId);

            existingEntities.Item2.FirstName = entity.FirstName;
            existingEntities.Item2.LastName = entity.LastName;

            await uow.UserProfiles.UpdateAsync(existingEntities.Item2);
            await uow.SaveAsync();

            return new UserProfileDTO()
            {
                ProfileId = existingEntities.Item2.ProfileId,
                Email = existingEntities.Item1.Email,
                FirstName = existingEntities.Item2.FirstName,
                LastName = existingEntities.Item2.LastName,
                JoiningDate = existingEntities.Item2.JoiningDate,
            };
        }

        public async Task DeleteAsync(int id)
        {
            var existingEntities = await CheckEntityPresence(id);

            await userManager.DeleteAsync(existingEntities.Item1);
        }

        public async Task<UserProfileDTO?> GetByIdAsync(int id)
        {
            var existingEntities = await CheckEntityPresence(id);

            return new UserProfileDTO()
            {
                ProfileId = existingEntities.Item2.ProfileId,
                Email = existingEntities.Item1.Email,
                FirstName = existingEntities.Item2.FirstName,
                LastName = existingEntities.Item2.LastName,
                JoiningDate = existingEntities.Item2.JoiningDate,
            };
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        private void ValidateEntity(UserProfileDTO entity)
        {
            if (entity.FirstName.IsNullOrEmpty() || entity.FirstName.Length > 20)
                throw new ValidationException(entity.GetType().Name, nameof(entity.FirstName));

            if (!entity.LastName.IsNullOrEmpty() && entity.LastName.Length > 20)
                throw new ValidationException(entity.GetType().Name, nameof(entity.LastName));
        }

        private void ValidateEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]{2,}@[a-zA-Z]{2,}\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);

            if(!regex.IsMatch(email))
                throw new ValidationException(typeof(UserProfileDTO).Name, "Email", "Виникла помилка під час валідації електронної пошти");
        }

        private async Task<Tuple<User?, UserProfile?>> CheckEntityPresence(int id)
        {
            var existingEntities = await uow.UserProfiles.GetUserAndProfileByIdAsync(id);
            if (existingEntities.Item1 == null || existingEntities.Item2 == null)
                throw new EntityNotFoundException(typeof(UserProfileDTO).Name, id);

            return existingEntities;
        }
    }
}
