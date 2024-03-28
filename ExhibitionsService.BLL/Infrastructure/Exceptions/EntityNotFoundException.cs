namespace ExhibitionsService.BLL.Infrastructure.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string EntityType { get; protected set; }
        public int EntityId { get; protected set; }

        public EntityNotFoundException(string entityType, int entityId)
            : base($"Сутність типу '{entityType}' з ідентифікатором '{entityId}' не знайдена.")
        {
            EntityType = entityType;
            EntityId = entityId;
        }

        public EntityNotFoundException(string entityType, int entityId, string message) : base(message)
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }
}
