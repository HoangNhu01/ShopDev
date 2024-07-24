using System.Text.Json;
using AutoMapper;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShopDev.Constants.ErrorCodes;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.Order.ApplicationServices.CartModule.Abstract;
using ShopDev.Order.ApplicationServices.CartModule.Dtos;
using ShopDev.Order.ApplicationServices.Common;
using ShopDev.Order.ApplicationServices.Protos;
using StackExchange.Redis;
using static ShopDev.Order.ApplicationServices.Protos.ProductProto;

namespace ShopDev.Order.ApplicationServices.CartModule.Implements
{
    public class CartService : OrderServiceBase, ICartService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public CartService(
            ILogger<CartService> logger,
            IHttpContextAccessor httpContext,
            IConnectionMultiplexer connectionMultiplexer
        )
            : base(logger, httpContext)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task AddToCart(CartUpdateDto input)
        {
            IDatabase redisDb = _connectionMultiplexer.GetDatabase();
            var ipAdd = _httpContext.GetCurrentRemoteIpAddress();
            string cartKey = $"cart:{ipAdd}";
            string cartJson = await redisDb.StringGetAsync(cartKey);
            List<ProductDto>? cartItems = !string.IsNullOrEmpty(cartJson)
                ? JsonSerializer.Deserialize<List<ProductDto>>(cartJson)
                : [];

            // Lấy từ trong cache ra giỏ hàng
            // Gọi gRPC qua Inventory để lấy thông tin sản phẩm
            GrpcChannel grpcChannel = GrpcChannel.ForAddress("http://localhost:5102/");
            ProductProtoClient productProto = new(grpcChannel);
            var productResponse = productProto.FindById(
                new()
                {
                    Id = input.Id,
                    Quantity = input.Quantity,
                    SpuId = input.SpuId
                }
            );
            // Cập nhật nếu thêm sản phẩm đã có trong giỏ hàng
            ProductDto? cartItem = cartItems?.Find(x =>
                x.Id == input.Id && x.Spus.Exists(s => s.SpuId == input.SpuId)
            );
            if (cartItem is not null)
            {
                cartItem.Quantity += input.Quantity;
            }
            else
            {
                cartItems?.Add(_mapper.Map<ProductDto>(productResponse.Product));
            }
            string cartJsonConvert = JsonSerializer.Serialize(cartItems);
            var result = await redisDb.StringSetAsync(cartKey, cartJsonConvert);
        }

        public async Task RemoveFromCart(CartUpdateDto input)
        {
            var useClaims = _httpContext.HttpContext?.User?.Identity?.Name;
            var ipAdd = _httpContext.GetCurrentRemoteIpAddress();
            string cartKey = $"cart:{ipAdd}";
            IDatabase redisDb = _connectionMultiplexer.GetDatabase();
            string cartJson = await redisDb.StringGetAsync(cartKey);

            var cartItems =
                JsonSerializer.Deserialize<List<ProductDto>>(cartJson)
                ?? throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            ProductDto product =
                cartItems.Find(x => x.Id == input.Id && x.Spus.Exists(s => s.SpuId == input.SpuId))
                ?? throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            if (product.Quantity == 0)
            {
                cartItems.Remove(product);
            }
            product.Quantity -= input.Quantity;
            string cartJsonConvert = JsonSerializer.Serialize(cartItems);
            await redisDb.StringSetAsync(cartKey, cartJsonConvert);
        }

        public async Task<List<ProductDto>> ViewCart()
        {
            var useClaims = _httpContext.HttpContext?.User.Identity?.Name;
            var ipAdd = _httpContext.GetCurrentRemoteIpAddress();
            string cartKey = $"cart:{ipAdd}";

            IDatabase redisDb = _connectionMultiplexer.GetDatabase();
            string cartJson = await redisDb.StringGetAsync(cartKey);

            List<ProductDto>? cartItems = !string.IsNullOrEmpty(cartJson)
                ? JsonSerializer.Deserialize<List<ProductDto>>(cartJson)
                    ?? throw new UserFriendlyException(OrderErrorCode.ProductNotFound)
                : [];
            return cartItems;
        }
    }
}
