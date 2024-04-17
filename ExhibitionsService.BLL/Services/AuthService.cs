using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace ExhibitionsService.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork uow;
        private readonly UserManager<User> userManager;

        public const string LoginProviderName = "ExhibitionService";
        public const string TokenName = "RefreshToken";

        public AuthService(IUnitOfWork _uow, UserManager<User> _userManager)
        {
            uow = _uow;
            userManager = _userManager;
        }

        public async Task<AuthorizationDataDTO> LoginAsync(UserProfileDTO entity, IConfiguration _config)
        {
            ValidateEntity(entity);

            var user = await userManager.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email == entity.Email);

            if (user == null || !(await userManager.CheckPasswordAsync(user, entity.Password)))
                throw new ValidationException("Неправильна електронна пошта чи пароль.");

            var claims = await GetClaims(user);
            var accessToken = GetToken(claims, DateTime.Now.AddMinutes(30), _config);
            var refreshToken = GetToken(claims, DateTime.Now.AddDays(7), _config);

            await userManager.SetAuthenticationTokenAsync(user, LoginProviderName, TokenName, refreshToken);

            return new AuthorizationDataDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ProfileId = user.UserProfile.ProfileId,
                Email = user.Email,
                Roles = (List<string>)(await userManager.GetRolesAsync(user))
            };
        }

        public async Task<AuthorizationDataDTO> RefreshTokenAsync(AuthorizationDataDTO entity, IConfiguration _config)
        {
            if (entity.AccessToken.IsNullOrEmpty())
                throw new ValidationException(entity.GetType().Name, nameof(entity.AccessToken), "Токен доступу відсунтій");

            if (entity.RefreshToken.IsNullOrEmpty())
                throw new ValidationException(entity.GetType().Name, nameof(entity.RefreshToken), "Рефреш токен відсунтій");

            ClaimsPrincipal? principal = ValidateExparedToken(entity.AccessToken, _config);
            if (principal == null)
                throw new ValidationException("Токен доступу не валідний.");

            var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim == null)
                throw new ValidationException("Токен доступу не містить інформації про email.");

            var user = await userManager.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email == emailClaim.Value);
            if(user == null)
                throw new ValidationException("Токен доступу містить не валідний email.");

            if (await userManager.GetAuthenticationTokenAsync(user, LoginProviderName, TokenName) != entity.RefreshToken)
                throw new ValidationException("Рефреш токен не валідний.");

            var claims = await GetClaims(user);
            var accessToken = GetToken(claims, DateTime.Now.AddMinutes(30), _config);
            var refreshToken = GetToken(claims, DateTime.Now.AddDays(7), _config);

            await userManager.SetAuthenticationTokenAsync(user, LoginProviderName, TokenName, refreshToken);

            return new AuthorizationDataDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ProfileId = user.UserProfile.ProfileId,
                Email = user.Email,
                Roles = (List<string>)(await userManager.GetRolesAsync(user))
            };
        }

        public async Task<AuthorizationDataDTO> GetAuthInfoAsync(Claim? profileIdClaim)
        {
            if (profileIdClaim == null)
                throw new ValidationException("Токен містить не коректну інформацію");

            int profileId = Int32.Parse(profileIdClaim.Value);
            var user = await userManager.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.UserProfile.ProfileId == profileId);
            if (user == null)
                throw new ValidationException("Токен доступу містить не валідний ідентифікатор профілю.");

            return new AuthorizationDataDTO
            {
                ProfileId = user.UserProfile.ProfileId,
                Email = user.Email,
                Roles = (List<string>)(await userManager.GetRolesAsync(user))
            };
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("ProfileId", user.UserProfile.ProfileId.ToString()),
            };

            var userRoles = await userManager.GetRolesAsync(user);
            foreach(var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private string GetToken(List<Claim> claims, DateTime expires, IConfiguration _config)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var accessToken = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        private void ValidateEntity(UserProfileDTO entity)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]{2,}@[a-zA-Z]{2,}\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);

            if (!regex.IsMatch(entity.Email))
                throw new ValidationException(typeof(UserProfileDTO).Name, "Email", "Виникла помилка під час валідації електронної пошти");

            if (entity.Password.IsNullOrEmpty())
                throw new ValidationException(entity.GetType().Name, nameof(entity.Password));
        }

        public ClaimsPrincipal? ValidateExparedToken(string token, IConfiguration _config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
            };

            try
            {
                SecurityToken securityToken;
                ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }
            catch (Exception)
            {
                throw new ValidationException("Токен доступу не валідний.");
            }
        }
    }
}
