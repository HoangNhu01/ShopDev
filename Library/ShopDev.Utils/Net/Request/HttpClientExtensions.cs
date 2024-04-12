using System.Text;
using System.Text.Json;

namespace ShopDev.Utils.Net.Request
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(
            this HttpClient httpClient,
            string requestUri,
            T data
        ) =>
            httpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data) }
            );

        public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(
            this HttpClient httpClient,
            string requestUri,
            T data,
            CancellationToken cancellationToken
        ) =>
            httpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data) },
                cancellationToken
            );

        public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(
            this HttpClient httpClient,
            Uri requestUri,
            T data
        ) =>
            httpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data) }
            );

        public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(
            this HttpClient httpClient,
            Uri requestUri,
            T data,
            CancellationToken cancellationToken
        ) =>
            httpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data) },
                cancellationToken
            );

        private static HttpContent Serialize(object? data) =>
            new StringContent(
                JsonSerializer.Serialize(data ?? new()),
                Encoding.UTF8,
                "application/json"
            );
    }
}
