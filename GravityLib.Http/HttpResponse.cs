using System.Net;
using System.Text.Json;
using GravityLib.Common.Errors;

namespace GravityLib.Http
{
    public class HttpResponse
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public HttpResponse(JsonSerializerOptions serializerSettings = null)
        {
            _serializerOptions = serializerSettings;
        }

        public string Content { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccessStatusCode => ((int)StatusCode >= 200 && (int)StatusCode <= 299);

        public ErrorModel Error => IsSuccessStatusCode ? null : Parse<ErrorModel>();

        public T Parse<T>()
        {
            return !string.IsNullOrWhiteSpace(Content)
                ? JsonSerializer.Deserialize<T>(Content, _serializerOptions)
                : default;
        }
    }

    public class HttpResponse<TData> : HttpResponse
    {
        public HttpResponse(JsonSerializerOptions serializerSettings = null)
            : base(serializerSettings)
        {
        }

        public TData Data => IsSuccessStatusCode ? Parse<TData>() : default;
    }
}
