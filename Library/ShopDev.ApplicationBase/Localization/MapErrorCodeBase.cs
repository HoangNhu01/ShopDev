using System.Reflection;
using ShopDev.EntitiesBase.Base;
using Microsoft.AspNetCore.Http;
using ShopDev.ApplicationBase.Localization;

namespace ShopDev.ApplicationBase.Localization
{
    public abstract class MapErrorCodeBase<TErrorCode> : IMapErrorCode
        where TErrorCode : IErrorCode
    {
        /// <summary>
        /// Tiền tố trong file xml translate
        /// </summary>
        protected virtual string PrefixError { get; } = "error_";
        private readonly Dictionary<int, string> Map = new();

        protected readonly ILocalization _localization;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Khởi tạo map error code và message
        /// </summary>
        /// <param name="localization"></param>
        /// <param name="httpContext"></param>
        /// <exception cref="InvalidOperationException"></exception>
        protected MapErrorCodeBase(ILocalization localization, IHttpContextAccessor httpContext)
        {
            var errorCodes = typeof(TErrorCode)
                .GetFields(
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                )
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .ToList();
            foreach (var errorCode in errorCodes)
            {
                var type = typeof(TErrorCode);
                int? code = (int?)type.GetField(errorCode.Name)?.GetRawConstantValue();
                while (code == null)
                {
                    var baseType = type.BaseType;
                    if (baseType is null)
                    {
                        break;
                    }
                    code = (int?)baseType.GetField(errorCode.Name)?.GetRawConstantValue();
                    type = baseType;
                }

                var messageKey = PrefixError + errorCode.Name;
                if (code != null)
                {
                    //thêm mã lỗi vào map có dạng Map[404] = "error_NotFound"
                    Map[code.Value] = messageKey;
                }
            }
            _localization = localization;
            _httpContextAccessor = httpContext;

            //var enums = (TErrorCode[])Enum.GetValues(typeof(TErrorCode));
            //if (errorCodes.Count != errorCodes.Distinct().Count())
            //{
            //    throw new InvalidOperationException($"enum {nameof(TErrorCode)} has duplicate value");
            //}
        }

        public string GetErrorMessageKey(int errorCode)
        {
            Map.TryGetValue(errorCode, out string? messageKey);
            return messageKey
                ?? throw new InvalidOperationException(
                    $"Not found messageKey for errorCode: {errorCode}"
                );
        }

        public string GetErrorMessage(int errorCode)
        {
            return _localization.Localize(GetErrorMessageKey(errorCode));
        }
    }
}
