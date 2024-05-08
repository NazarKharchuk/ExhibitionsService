using AutoMapper;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.PL.Models.HelperModel;

namespace ExhibitionsService.PL.Mapping.Helper
{
    public class StatisticsResponseModelSProfiles : Profile
    {
        public StatisticsResponseModelSProfiles()
        {
            CreateMap<StatisticsLikesValueDTO, StatisticsLikesValueModel>();
            CreateMap<RatingValueDTO, RatingValueModel>();
            CreateMap<StatisticsRatingsValueDTO, StatisticsRatingsValueModel>();
            CreateMap<StatisticsResponseDTO<StatisticsLikesValueDTO>, StatisticsResponseModel<StatisticsLikesValueModel>>();
            CreateMap<StatisticsResponseDTO<StatisticsRatingsValueDTO>, StatisticsResponseModel<StatisticsRatingsValueModel>>();
        }
    }
}
