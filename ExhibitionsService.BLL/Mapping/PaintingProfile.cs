using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class PaintingProfile : Profile
    {
        public PaintingProfile()
        {
            CreateMap<Painting, PaintingDTO>().ReverseMap();
        }
    }
}
