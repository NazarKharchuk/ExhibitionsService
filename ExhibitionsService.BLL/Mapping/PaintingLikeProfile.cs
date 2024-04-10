using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class PaintingLikeProfile : Profile
    {
        public PaintingLikeProfile()
        {
            CreateMap<PaintingLike, PaintingLikeDTO>().ReverseMap();
        }
    }
}
