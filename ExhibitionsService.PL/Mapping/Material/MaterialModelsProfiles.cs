using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.PL.Models.Material;

namespace ExhibitionsService.PL.Mapping.Material
{
    public class MaterialModelsProfiles : Profile
    {
        public MaterialModelsProfiles()
        {
            CreateMap<MaterialModel, MaterialDTO>().ReverseMap();
            CreateMap<MaterialCreateModel, MaterialDTO>().ReverseMap();
            CreateMap<MaterialUpdateModel, MaterialDTO>().ReverseMap();
        }
    }
}
