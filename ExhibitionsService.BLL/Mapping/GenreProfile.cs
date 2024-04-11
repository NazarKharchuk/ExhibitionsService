using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class GenreProfile : Profile
    {
        public GenreProfile()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
        }
    }
}