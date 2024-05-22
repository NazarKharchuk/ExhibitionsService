using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Enums;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Security.Claims;
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

        public async Task<UserProfileDTO> UpdateAsync(UserProfileDTO entity, ClaimsPrincipal claims)
        {
            ValidateEntity(entity);

            string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
            int profileId = profileIdClaim != null ?
                int.Parse(profileIdClaim) :
                throw new ValidationException("Користувач не авторизований");
            if (profileId != entity.ProfileId)
                throw new InsufficientPermissionsException("Редагувати профіль користувача може тільки його автор");

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

        public async Task DeleteAsync(int id, ClaimsPrincipal claims)
        {
            var roles = claims.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!roles.Contains("Admin"))
            {
                string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
                int profileId = profileIdClaim != null ? int.Parse(profileIdClaim) :
                        throw new ValidationException("Користувач не авторизований чи не має ролі адміністратора");

                if (profileId != id)
                    throw new InsufficientPermissionsException("Видалити профіль користувача може тільки його власник чи адміністратор");
            }

            var existingEntities = await CheckEntityPresence(id);

            using (var transaction = await uow.BeginTransactionAsync())
            {
                try
                {
                    await uow.UserProfiles.DeleteRelatedInfo(id);

                    var result = await userManager.DeleteAsync(existingEntities.Item1);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Не вдалося видалити профіль");
                    }

                    await uow.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception($"Помилка на етапі створення видалення профіля({e.Message})");
                }
            }
        }

        public async Task AddRole(int id, Role _role)
        {
            var existingEntities = await CheckEntityPresence(id);
            var role = _role.ToString();

            var userHasRole = await userManager.IsInRoleAsync(existingEntities.Item1, role);
            if (userHasRole)
            {
                throw new ValidationException($"Користувач вже має роль '{role}'.");
            }

            var result = await userManager.AddToRoleAsync(existingEntities.Item1, role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new Exception($"Не вдалось додати роль'{role}' користувачеві. Помилки: {string.Join(", ", errors)}");
            }
        }

        public async Task DeleteRole(int id, Role _role)
        {
            var existingEntities = await CheckEntityPresence(id);
            var role = _role.ToString();

            var userHasRole = await userManager.IsInRoleAsync(existingEntities.Item1, role);
            if (!userHasRole)
            {
                throw new ValidationException($"Користувач ще не має ролі художника.");
            }

            var result = await userManager.RemoveFromRoleAsync(existingEntities.Item1, role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new Exception($"Не вдалось видалити роль'{role}' користувачеві. Помилки: {string.Join(", ", errors)}");
            }
        }

        public async Task<UserProfileDTO?> GetByIdAsync(int id, ClaimsPrincipal claims)
        {
            var roles = claims.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!roles.Contains("Admin"))
            {
                string? profileIdClaim = claims.FindFirst("ProfileId")?.Value;
                int profileId = profileIdClaim != null ? int.Parse(profileIdClaim) :
                        throw new ValidationException("Користувач не авторизований чи не має ролі адміністратора");

                if (profileId != id)
                    throw new InsufficientPermissionsException("Переглянути профіль користувача може тільки його власник чи адміністратор");
            }

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

        public async Task<Tuple<List<UserProfileInfoDTO>, int>> GetPageUserProfilesAsync(PaginationRequestDTO pagination)
        {
            var all = await uow.UserProfiles.GetAllProfilesWithUsersAsync();
            int count = all.Count();
            if (pagination.PageNumber == null) pagination.PageNumber = 1;
            if (pagination.PageSize == null) { pagination.PageSize = 10; };
            pagination.PageSize = Math.Min(pagination.PageSize.Value, 20);
            if (pagination.PageNumber < 1 ||
                pagination.PageNumber < 1 ||
                (pagination.PageNumber > (int)Math.Ceiling((double)count / pagination.PageSize.Value) && count != 0))
            {
                throw new ValidationException("Не коректний номер або розмір сторінки.");
            }

            all = all.Skip((int)((pagination.PageNumber - 1) * pagination.PageSize)).Take((int)pagination.PageSize);

            var page = await all.ToListAsync();

            var userProfileInfoDtos = new List<UserProfileInfoDTO>();
            foreach (var userProfile in page)
            {
                var userProfileInfoDto = new UserProfileInfoDTO
                {
                    ProfileId = userProfile.ProfileId,
                    Email = userProfile.User.Email,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    JoiningDate = userProfile.JoiningDate,
                    Roles = await GetRolesForUserAsync(userProfile.User.Id),
                };
                userProfileInfoDtos.Add(userProfileInfoDto);
            }

            return Tuple.Create(userProfileInfoDtos, count);
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

        private async Task<List<string>> GetRolesForUserAsync(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new EntityNotFoundException(typeof(UserProfileDTO).Name, userId);

            var roles = await userManager.GetRolesAsync(user);

            return [.. roles];
        }
    }
}
