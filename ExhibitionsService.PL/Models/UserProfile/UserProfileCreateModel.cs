using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.UserProfile
{
    public class UserProfileCreateModel
    {
        [Required(ErrorMessage = "Поле 'Email' профіля користувача є обов'язковим.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле 'FirstName' профіля користувача є обов'язковим.")]
        [MaxLength(20, ErrorMessage = "Довжина поля 'FirstName' профіля користувача не повинна перевищувати 20 символів.")]
        public string FirstName { get; set; }

        [MaxLength(20, ErrorMessage = "Довжина поля 'LastName' профіля користувача не повинна перевищувати 20 символів.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Поле 'Password' профіля користувача є обов'язковим.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
