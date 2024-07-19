namespace ShopDev.Constants.Common.Cache
{
    public static class PrefixKeyCache
    {
        public const string Demo = "demo-";
        public const string WildCard = "*";

        #region prefix key auth
        public const string Auth = "auth-";
        public const string AuthUser = $"{Auth}user-";
        public const string AuthToken = $"{Auth}token-";

        //Dùng cho node để check token đã bị thu hồi chưa (trong trường hợp logout nhưng token chưa hết hạn)
        public const string AuthTokenRevoke = $"{Auth}revoke-token-";

        /// <summary>
        /// Dùng cho fcm token lưu ở redis
        /// </summary>
        public const string NotificationAuthToken = $"notification-auth-token-";
        #endregion

        #region prefix key inventory
        public const string Inventory = "inventory-";
        public const string Product = $"{Inventory}product-";
        #endregion

        #region prefix key order
        public const string Order = "order-";

        /// <summary>
        /// Dành cho api find by id
        /// </summary>
        public const string Cart = $"{Order}cart-";
        #endregion

        /// <summary>
        /// Nối chuỗi prefixKey dành cho auth token
        /// </summary>
        /// <param name="authorizationId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public static string AuthTokenString(string authorizationId, string tokenId)
        {
            return AuthToken + authorizationId + "-" + tokenId;
        }

        /// <summary>
        /// Pattern xóa cache
        /// </summary>
        /// <param name="prefixKey"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public static string PatternString(string prefixKey, object? keyId)
        {
            return prefixKey + keyId + "*";
        }

        /// <summary>
        /// Pattern xóa cache
        /// </summary>
        /// <param name="prefixKey"></param>
        /// <param name="keyId"></param>
        /// <param name="prefixKey2"></param>
        /// <returns></returns>
        public static string PatternString(string prefixKey, object keyId, string prefixKey2)
        {
            return prefixKey + keyId + "-" + prefixKey2 + "*";
        }
    }
}
