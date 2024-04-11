using Microsoft.EntityFrameworkCore.Storage;

namespace ExhibitionsService.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserProfileRepository UserProfiles { get; }
        IPainterRepository Painters { get; }
        IPaintingRepository Paintings { get; }
        IImageRepository Images { get; }
        IPaintingRatingRepository PaintingRatings { get; }
        IExhibitionRepository Exhibitions { get; }
        IExhibitionApplicationRepository ExhibitionApplications { get; }
        IContestRepository Contests { get; }
        IContestApplicationRepository ContestApplications { get; }
        IGenreRepository Genres { get; }
        IStyleRepository Styles { get; }
        IMaterialRepository Materials { get; }
        ITagRepository Tags { get; }

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveAsync();

        void Dispose();
    }
}
