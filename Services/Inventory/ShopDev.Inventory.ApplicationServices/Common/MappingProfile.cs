using AutoMapper;
using ShopDev.Inventory.ApplicationServices.CategoryModule.Dtos;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;
using ShopDev.Inventory.ApplicationServices.ShopModule.Dtos;
using ShopDev.Inventory.Domain.Categories;
using ShopDev.Inventory.Domain.Products;
using ShopDev.Inventory.Domain.Shops;

namespace ShopDev.Authentication.ApplicationServices.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VariationUpdateDto, Variation>().ReverseMap();
            CreateMap<AttributeUpdateDto, AttributeType>().ReverseMap();
            CreateMap<CategoryDetailDto, Category>().ReverseMap();
            CreateMap<ShopDetailDto, Shop>().ReverseMap();
        }
    }
}
