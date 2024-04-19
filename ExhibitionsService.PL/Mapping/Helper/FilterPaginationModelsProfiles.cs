using AutoMapper;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.PL.Models.HelperModel;

namespace ExhibitionsService.PL.Mapping.Helper
{
    public class FilterPaginationModelsProfiles : Profile
    {
        public FilterPaginationModelsProfiles()
        {
            CreateMap<PaginationRequestModel, PaginationRequestDTO>();
        }
    }
}
