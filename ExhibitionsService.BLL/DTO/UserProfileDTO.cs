namespace ExhibitionsService.BLL.DTO
{
    public class UserProfileDTO
    {
        public int ProfileId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? Password { get; set; }
    }
}
