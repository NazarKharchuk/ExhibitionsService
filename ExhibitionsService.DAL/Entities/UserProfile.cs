namespace ExhibitionsService.DAL.Entities
{
    public class UserProfile
    {
        public int ProfileId {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime JoiningDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
