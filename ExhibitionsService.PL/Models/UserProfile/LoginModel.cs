using System.ComponentModel.DataAnnotations;

namespace ExhibitionsService.PL.Models.UserProfile
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Поле 'Email' є обов'язковим.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле 'Password' є обов'язковим.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
