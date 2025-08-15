
using AutoMapper;
using E_Commerce.Business.ViewModels.Product;
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

            CreateMap<ProductAddViewModel, Product>()
            .ForMember(dest => dest.ProductImages, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.MainImageUrl, opt => 
                opt.MapFrom(src => src.MainImageUrl ?? string.Empty))
            // Ignore base entity properties
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ReverseMap();

        }
    }
}
