using AutoMapper;
using ShopDev.Order.ApplicationServices.CartModule.Dtos;
using ShopDev.Order.ApplicationServices.OrderModule.Dtos;
using ShopDev.Order.ApplicationServices.Protos;
using ShopDev.Order.Domain.Order;

namespace ShopDev.Order.ApplicationServices.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Spu, SpuDto>().ReverseMap();
            CreateMap<OrderGen, OrderDto>().ReverseMap();
            CreateMap<Domain.Products.Product, ProductDto>().ReverseMap();
            CreateMap<Domain.Products.Spu, SpuDto>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailDto>().ReverseMap();
        }
    }
}
