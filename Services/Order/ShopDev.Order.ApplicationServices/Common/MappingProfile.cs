using AutoMapper;
using ShopDev.Order.ApplicationServices.CartModule.Dtos;
using ShopDev.Order.ApplicationServices.Protos;

namespace ShopDev.Order.ApplicationServices.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Spu, SpuDto>().ReverseMap();
        }
    }
}
