using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.DAL.Entities;

namespace ExhibitionsService.BLL.Mapping
{
    public class PaintingProfile : Profile
    {
        public PaintingProfile()
        {
            CreateMap<Painting, PaintingDTO>().ReverseMap();
            CreateMap<Painting, PaintingInfoDTO>()
                    .ForMember(dest => dest.PainterId, opt => opt.MapFrom(src => src.Painter.PainterId))
                    .ForMember(dest => dest.Painter, opt => opt.MapFrom(src => src.Painter))
                    .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres))
                    .ForMember(dest => dest.Styles, opt => opt.MapFrom(src => src.Styles))
                    .ForMember(dest => dest.Materials, opt => opt.MapFrom(src => src.Materials))
                    .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
                    .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.PaintingLikes.Count))
                    .ForMember(dest => dest.IsLiked, opt => opt.Ignore())
                    .ForMember(dest => dest.RatingCount, opt => opt.MapFrom(src => src.Ratings.Count))
                    .ForMember(dest => dest.AvgRating, opt => opt.MapFrom(src =>
                        src.Ratings.Count > 0 ? src.Ratings.Average(r => r.RatingValue) : 0))
                    .ForMember(dest => dest.ExhibitionApplicationsCount, opt => opt.MapFrom(src =>
                        src.ExhibitionApplications.Count))
                    .ForMember(dest => dest.ContestApplicationsCount, opt => opt.MapFrom(src =>
                        src.ContestApplications.Count))
                    .ForMember(dest => dest.ContestVictoriesCount, opt => opt.MapFrom(src =>
                        src.ContestApplications.Count(ca => ca.IsWon)));

        }
    }
}
