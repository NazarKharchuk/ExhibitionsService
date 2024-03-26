using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserProfileRepository UserProfiles { get; }
        Task SaveAsync();

        void Dispose();
    }
}
