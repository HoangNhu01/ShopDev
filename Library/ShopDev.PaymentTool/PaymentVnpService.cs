using System.Net;
using System.Text;
using ShopDev.PaymentTool.Interfaces;
using ShopDev.Utils.PaymentTool;
using ShopDev.Utils.Security;

namespace ShopDev.PaymentTool
{
    public class PaymentVnpService : IPaymentVnpService
    {
        private SortedList<string, string> _requestData;
        private SortedList<string, string> _responseData;

        public PaymentVnpService()
        {
            PayCompare compare = new();
            _requestData = new(compare);
            _responseData = new(compare);
        }

        public void AddResponseData(string key, string value)
        {
            _responseData.Add(key, value);
        }

        /// <summary>
        /// Tạo Url về bên thứ 3
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="vnp_HashSecret"></param>
        /// <returns></returns>
        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(
                        WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&"
                    );
                }
            }
            string queryString = data.ToString();

            baseUrl += "?" + queryString;
            string signData = queryString;
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }
            string vnp_SecureHash = CryptographyUtils.HmacSHA512(vnp_HashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;
            return baseUrl;
        }

        /// <summary>
        /// Láy data trả về theo key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? GetResponseDataByKey(string key)
        {
            if (_responseData.TryGetValue(key, out string? retValue))
            {
                return retValue;
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Lấy data trả về
        /// </summary>
        /// <returns></returns>
        public string GetResponseData()
        {
            StringBuilder data = new();
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }
            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }
            foreach (KeyValuePair<string, string> kv in _responseData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(
                        WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&"
                    );
                }
            }
            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }
            return data.ToString();
        }
    }
}
