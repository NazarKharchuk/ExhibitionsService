using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class PainterProfile : Profile
    {
        public PainterProfile()
        {
            CreateMap<Painter, PainterDTO>().ReverseMap();
            CreateMap<Painter, PainterInfoDTO>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.UserProfile.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.UserProfile.LastName))
                .ForMember(dest => dest.JoiningDate, opt => opt.MapFrom(src => src.UserProfile.JoiningDate))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src =>
                    src.Paintings.Any() ? src.Paintings.Sum(pg => pg.PaintingLikes.Count) : 0))
                .ForMember(dest => dest.VictoriesCount, opt => opt.MapFrom(src =>
                    src.Paintings.Any() ? src.Paintings.SelectMany(pg => pg.ContestApplications).Count(ca => ca.IsWon) : 0))
                .ForMember(dest => dest.RatingCount, opt => opt.MapFrom(src =>
                    src.Paintings.Any() ? src.Paintings.Sum(pg => pg.Ratings.Count) : 0))
                .ForMember(dest => dest.AvgRating, opt => opt.MapFrom(src =>
                    src.Paintings.Any() && src.Paintings.SelectMany(painting => painting.Ratings).Any()
                        ? src.Paintings.SelectMany(painting => painting.Ratings).Average(r => r.RatingValue) : 0));
        }
    }
}
