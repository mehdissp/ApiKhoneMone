using AutoMapper;
using JWTApi.Domain.Dtos.RealEstate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Helper
{
    public class Mappers
    {

    }
    public class RealEstateMappingProfile : Profile
    {
        public RealEstateMappingProfile()
        {
            CreateMap<JWTApi.Domain.Dtos.RealEstate.RealEstateDto,
                      JWTApi.Application.DTOs.RealEstates.RealEstateDto>()
                .ReverseMap(); // اگر دوطرفه نیاز دارید
        }
    }
    public class CategorireMappingProfile : Profile
    {
        public CategorireMappingProfile()
        {
            CreateMap<JWTApi.Domain.Dtos.Categories.CategoryDto,
                      JWTApi.Application.DTOs.Categories.CategoryDto>()
                .ReverseMap(); // اگر دوطرفه نیاز دارید
        }
    }
    public class RealEstateMappingWithCategoryProfile : Profile
    {
        public RealEstateMappingWithCategoryProfile()
        {
            CreateMap<RealEstateWithCategoryDto, RealEstateDto>()
                .ForMember(dest => dest.ImageCount,
                          opt => opt.MapFrom(src => src.ImageCount));
         
        }
    }
}
