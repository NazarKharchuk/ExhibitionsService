namespace ExhibitionsService.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserProfileRepository UserProfiles { get; }
        IPainterRepository Painters { get; } 
        Task SaveAsync();

        void Dispose();
    }
}
