using System.Text.Json;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShopDev.Constants.ErrorCodes;
using ShopDev.InfrastructureBase.Exceptions;
using ShopDev.Order.ApplicationServices.CartModule.Abstract;
using ShopDev.Order.ApplicationServices.CartModule.Dtos;
using ShopDev.Order.ApplicationServices.Common;
using ShopDev.Order.ApplicationServices.OrderModule.Abstracts;
using ShopDev.Order.ApplicationServices.OrderModule.Dtos;
using StackExchange.Redis;
using static ShopDev.Order.ApplicationServices.Protos.ProductProto;

namespace ShopDev.Order.ApplicationServices.CartModule.Implements
{
    public class CartService : OrderServiceBase, ICartService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IOrderService _orderService;

        public CartService(
            ILogger<CartService> logger,
            IHttpContextAccessor httpContext,
            IConnectionMultiplexer connectionMultiplexer,
            IOrderService orderService
        )
            : base(logger, httpContext)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _orderService = orderService;
        }

        public async Task AddToCart(CartUpdateDto input)
        {
            IDatabase redisDb = _connectionMultiplexer.GetDatabase();
            var ipAdd = _httpContext.GetCurrentRemoteIpAddress();
            string cartKey = $"cart:{ipAdd}";
            string? cartJson = await redisDb.StringGetAsync(cartKey);
            List<CartItemDto>? cartItems = !string.IsNullOrEmpty(cartJson)
                ? JsonSerializer.Deserialize<List<CartItemDto>>(cartJson)
                : [];
            CartItemDto? cartItem = cartItems?.Find(x =>
                x.Id == input.Id && x.SpuId == input.SpuId
            );
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
            var productOrg = _mapper.Map<CartItemDto>(productResponse.Product);
            if (productOrg.Quantity < (cartItem?.Quantity ?? 0) + input.Quantity)
            {
                throw new UserFriendlyException(OrderErrorCode.ProductIsNotEnough);
            }
            // Cập nhật nếu thêm sản phẩm đã có trong giỏ hàng
            if (cartItem is not null)
            {
                cartItem.Quantity += input.Quantity;
            }
            else
            {
                productOrg.Quantity = input.Quantity;
                cartItems?.Add(productOrg);
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
            string? cartJson = await redisDb.StringGetAsync(cartKey);
            if (string.IsNullOrWhiteSpace(cartJson))
                return;
            var cartItems =
                JsonSerializer.Deserialize<List<CartItemDto>>(cartJson)
                ?? throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            CartItemDto product =
                cartItems.Find(x => x.Id == input.Id && x.SpuId == input.SpuId)
                ?? throw new UserFriendlyException(InventoryErrorCode.ProductNotFound);
            product.Quantity -= input.Quantity;
            if (input.Quantity == 0)
            {
                cartItems.Remove(product);
            }
            if (!cartItems.Any())
            {
                _ = await redisDb.KeyDeleteAsync(cartKey);
                return;
            }
            string cartJsonConvert = JsonSerializer.Serialize(cartItems);
            await redisDb.StringSetAsync(cartKey, cartJsonConvert);
        }

        public async Task<List<CartItemDto>> ViewCart()
        {
            var useClaims = _httpContext.HttpContext?.User.Identity?.Name;
            var ipAdd = _httpContext.GetCurrentRemoteIpAddress();
            string cartKey = $"cart:{ipAdd}";

            IDatabase redisDb = _connectionMultiplexer.GetDatabase();
            string? cartJson = await redisDb.StringGetAsync(cartKey);

            var cartItems = !string.IsNullOrEmpty(cartJson)
                ? JsonSerializer.Deserialize<List<CartItemDto>>(cartJson)
                    ?? throw new UserFriendlyException(OrderErrorCode.ProductNotFound)
                : [];
            return cartItems;
        }

        public async Task CheckOutCart(OrderCreateDto input)
        {
            var ipAdd = _httpContext.GetCurrentRemoteIpAddress();
            string cartKey = $"cart:{ipAdd}";

            IDatabase redisDb = _connectionMultiplexer.GetDatabase();
            string? cartJson = await redisDb.StringGetAsync(cartKey);
            var cartItems = !string.IsNullOrEmpty(cartJson)
                ? JsonSerializer.Deserialize<List<CartItemDto>>(cartJson)
                    ?? throw new UserFriendlyException(OrderErrorCode.ProductNotFound)
                : [];
            foreach (var item in input.CartItems)
            {
                var cart = cartItems.Find(x => x.Id == item.Id);
                if (cart is not null)
                {
                    cartItems.Remove(cart);
                }
            }
            if (input.CartItems.Count == 0)
            {
                input.CartItems = cartItems;
            }
            await _orderService.Create(input);
        }
    }
}
