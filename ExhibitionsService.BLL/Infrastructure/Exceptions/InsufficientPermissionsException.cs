namespace ExhibitionsService.BLL.Infrastructure.Exceptions
{
    public class InsufficientPermissionsException : Exception
    {
        public InsufficientPermissionsException(string? message) : base(message) { }
    }
}
