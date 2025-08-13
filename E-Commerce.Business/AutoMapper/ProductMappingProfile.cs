
using AutoMapper;
using E_Commerce.Business.ViewModels;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.Business.AutoMapper
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.InStock))
                .ForMember(dest => dest.EffectivePrice, opt => opt.MapFrom(src => src.EffectivePrice))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AverageRating))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.ReviewCount));
        }
    }
}
