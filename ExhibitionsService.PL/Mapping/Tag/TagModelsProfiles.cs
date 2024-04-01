using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.PL.Models.Tag;

namespace ExhibitionsService.PL.Mapping.Tag
{
    public class TagModelsProfiles : Profile
    {
        public TagModelsProfiles()
        {
            CreateMap<TagModel, TagDTO>().ReverseMap();
            CreateMap<TagCreateModel, TagDTO>().ReverseMap();
            CreateMap<TagUpdateModel, TagDTO>().ReverseMap();
        }
    }
}
