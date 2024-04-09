using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.PL.Models.ExhibitionApplication;

namespace ExhibitionsService.PL.Mapping.ExhibitionApplication
{
    public class ExhibitionApplicationModelsProfiles : Profile
    {
        public ExhibitionApplicationModelsProfiles()
        {
            CreateMap<ExhibitionApplicationModel, ExhibitionApplicationDTO>().ReverseMap();
            CreateMap<ExhibitionApplicationCreateModel, ExhibitionApplicationDTO>().ReverseMap();
        }
    }
}
