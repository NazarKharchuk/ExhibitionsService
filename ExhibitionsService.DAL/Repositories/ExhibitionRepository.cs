using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace ExhibitionsService.DAL.Repositories
{
    public class ExhibitionRepository: RepositoryBase<Exhibition>, IExhibitionRepository
    {
        public ExhibitionRepository(ExhibitionContext _db) : base(_db)
        {

        }
    }
}
