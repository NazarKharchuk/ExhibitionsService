﻿using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.PL.Models.Exhibition;

namespace ExhibitionsService.PL.Mapping.Exhibition
{
    public class ExhibitionModelsProfiles : Profile
    {
        public ExhibitionModelsProfiles()
        {
            CreateMap<ExhibitionModel, ExhibitionDTO>().ReverseMap();
            CreateMap<ExhibitionCreateModel, ExhibitionDTO>().ReverseMap();
            CreateMap<ExhibitionUpdateModel, ExhibitionDTO>().ReverseMap();
        }
    }
}
