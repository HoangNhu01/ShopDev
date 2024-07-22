using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using DocumentFormat.OpenXml.Office2010.Excel;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
            List<Product> cartItems = [];
            if (!string.IsNullOrEmpty(cartJson))
            {
                cartItems = JsonSerializer.Deserialize<List<Product>>(cartJson) ?? [];
            }
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
            int quantity = input.Quantity;
            Product? cartItem = cartItems.Find(x =>
                x.Id == input.Id && x.Spus.Any(s => s.SpuId == input.SpuId)
            );
            if (cartItem is not null)
            {
                quantity += cartItem.Quantity;
                cartItems.Remove(cartItem);
            }
            cartItems.Add(productResponse.Product);
            string cartJsonConvert = JsonSerializer.Serialize(cartItems);
            var result = await redisDb.StringSetAsync(cartKey, cartJsonConvert);
        }
    }
}
