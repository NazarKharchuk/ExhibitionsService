namespace ExhibitionsService.PL.Models.UserProfile
{
    public class UserProfileInfoModel
    {
        public int ProfileId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime JoiningDate { get; set; }
        public List<string> Roles { get; set; }
    }
}
