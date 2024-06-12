using AutoMapper;
using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;
using ShopDev.Inventory.Domain.Products;

namespace ShopDev.Authentication.ApplicationServices.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VariationUpdateDto, Variation>().ReverseMap();
            CreateMap<AttributeUpdateDto, AttributeType>().ReverseMap();
        }
    }
}
