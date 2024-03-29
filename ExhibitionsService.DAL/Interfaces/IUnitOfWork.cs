using Microsoft.EntityFrameworkCore.Storage;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserProfileRepository UserProfiles { get; }
        IPainterRepository Painters { get; }
        
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveAsync();

        void Dispose();
    }
}
