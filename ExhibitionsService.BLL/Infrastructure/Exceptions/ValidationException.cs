namespace ExhibitionsService.BLL.Infrastructure.Exceptions
{
    public class ValidationException : Exception
    {
        public string? EntityType { get; protected set; }
        public string? Property {  get; protected set; }

        public ValidationException(string entityType, string prop)
            : base($"Властивість '{prop}' сутності типу '{entityType}' не відповідає првилам валідації.")
        {
            EntityType = entityType;
            Property = prop;
        }

        public ValidationException(string entityType, string prop, string message) : base(message)
        {
            EntityType = entityType;
            Property = prop;
        }

        public ValidationException(string? message) : base(message)
        {
            EntityType = null;
            Property = null;
        }
    }
}
