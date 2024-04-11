using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.PL.Models.Genre;

namespace ExhibitionsService.PL.Mapping.Genre
{
    public class GenreModelsProfiles : Profile
    {
        public GenreModelsProfiles()
        {
            CreateMap<GenreModel, GenreDTO>().ReverseMap();
            CreateMap<GenreCreateModel, GenreDTO>().ReverseMap();
            CreateMap<GenreUpdateModel, GenreDTO>().ReverseMap();
        }
    }
}
