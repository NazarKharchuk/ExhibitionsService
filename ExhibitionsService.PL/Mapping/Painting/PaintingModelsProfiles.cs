﻿using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;
using ExhibitionsService.PL.Models.Painting;

namespace ExhibitionsService.PL.Mapping.Painting
{
    public class PaintingModelsProfiles : Profile
    {
        public PaintingModelsProfiles()
        {
            CreateMap<PaintingModel, PaintingDTO>().ReverseMap();
            CreateMap<PaintingCreateModel, PaintingDTO>().ReverseMap();
            CreateMap<PaintingUpdateModel, PaintingDTO>().ReverseMap();
            CreateMap<PaintingInfoDTO, PaintingInfoModel>();
        }
    }
}
