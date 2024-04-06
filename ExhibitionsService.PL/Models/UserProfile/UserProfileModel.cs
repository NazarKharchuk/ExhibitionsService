using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.UserProfile
{
    public class UserProfileModel
    {
        [Required(ErrorMessage = "Поле 'ProfileId' профіля користувача є обов'язковим.")]
        public int ProfileId { get; set; }

        [Required(ErrorMessage = "Поле 'Email' профіля користувача є обов'язковим.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле 'FirstName' профіля користувача є обов'язковим.")]
        [MaxLength(20, ErrorMessage = "Довжина поля 'FirstName' профіля користувача не повинна перевищувати 20 символів.")]
        public string FirstName { get; set; }

        [MaxLength(20, ErrorMessage = "Довжина поля 'LastName' профіля користувача не повинна перевищувати 20 символів.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Поле 'JoiningDate' профіля користувача є обов'язковим.")]
        public DateTime JoiningDate { get; set; }
    }
}
