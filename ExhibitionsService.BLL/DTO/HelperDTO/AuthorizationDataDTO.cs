namespace ExhibitionsService.BLL.DTO.HelperDTO
{
    public class AuthorizationDataDTO
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int ProfileId { get; set; }
        public int? PainterId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
