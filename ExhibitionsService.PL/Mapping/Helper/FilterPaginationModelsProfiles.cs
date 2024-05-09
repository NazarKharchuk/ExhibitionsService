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
            CreateMap<PaintingsFiltrationPaginationRequestModel, PaintingsFiltrationPaginationRequestDTO>();
            CreateMap<ContestsFiltrationPaginationRequestModel, ContestsFiltrationPaginationRequestDTO>();
            CreateMap<ExhibitionFiltrationPaginationRequestModel, ExhibitionFiltrationPaginationRequestDTO>();
            CreateMap<PaintersFiltrationPaginationRequestModel, PaintersFiltrationPaginationRequestDTO>();
        }
    }
}
