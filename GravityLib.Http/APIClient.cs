using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using GravityLib.Http.Config;
using GravityLib.Http.HttpRetry;
using Microsoft.Extensions.Logging;

namespace GravityLib.Http
{
    /// <summary>
    /// Base web client class.
    /// Implement common methods that inherited classes can reuse.
    /// </summary>
    public class APIClient : IAPIClient, IDisposable
    {
        private readonly ILogger _logger;

        protected JsonSerializerOptions serializerOptions;

        public APIClientConfig Config { get; private set; }

        public HttpClient HttpClient { get; private set; }

        private bool _usesExternalHttpClient = false;

        public event Action<HttpRequestMessage> OnBeforeRequest;

        static APIClient()
        {
            // Updates inspired by:
            // https://stackoverflow.com/questions/32994464/could-not-create-ssl-tls-secure-channel-despite-setting-servercertificatevalida
            // https://stackoverflow.com/questions/2859790/the-request-was-aborted-could-not-create-ssl-tls-secure-channel
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, error) => true;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Default constructor with an internally managed (created) HttpClient
        /// </summary>
        /// <param name="config"> The WebApi client configuration </param>
        /// <param name="logger"> The logger service </param>
        /// <param name="serializerOptions"> Provides a way to inject custom serializer settings </param>
        public APIClient(APIClientConfig config, ILogger logger = null, JsonSerializerOptions serializerOptions = null)
        {
            _logger = logger;
            Configure(config);
            SetSerializerOptions(serializerOptions);
        }

        /// <summary>
        /// Default constructor with an external HttpClient
        /// </summary>
        /// <param name="client"> An external HttpClient (usually injected via DI) </param>
        /// <param name="config"> The WebApi client configuration </param>
        /// <param name="logger"> The logger service </param>
        /// <param name="serializerOptions"> Provides a way to inject custom serializer settings </param>
        public APIClient(HttpClient client, APIClientConfig config, ILogger logger = null, JsonSerializerOptions serializerOptions = null)
            : this(config, logger, serializerOptions)
        {
            _usesExternalHttpClient = true;
            HttpClient = client;
        }
        
        private void SetSerializerOptions(JsonSerializerOptions serializerOptions = null)
        {
            if (serializerOptions != null)
            {
                this.serializerOptions = serializerOptions;
            }
            else
            {
                this.serializerOptions = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    PropertyNamingPolicy = null, // This option turns camelCase json return type to PascalCase.
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
                };
                this.serializerOptions.Converters.Add(new JsonStringEnumConverter());
            }
        }

        public void Dispose()
        {
            if (!_usesExternalHttpClient)
            {
                HttpClient?.Dispose();
            }
        }

        /// <inheritdoc />
        public void Configure(APIClientConfig config)
        {
            Config = config ?? throw new ArgumentException("API client configuration cannot be null.", nameof(config));
            HttpClient ??= new HttpClient(new HttpRetryMessageHandler(_logger, Config.RetryCount, Config.RetryInterval));

            if (Config.AuthType == AuthType.Basic)
            {
                var byteArray = Encoding.ASCII.GetBytes($"{Config.ClientId}:{Config.ClientSecret}");
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
            else
            if (Config.AuthType == AuthType.Token || Config.AuthType == AuthType.Bearer)
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Config.AuthType.ToString(), Config.Token);
            }

            HttpClient.BaseAddress = new Uri(Config.Url);
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            HttpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            HttpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }

        /// <inheritdoc />
        public async Task<HttpResponse> GetAsync(
            string route,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            OnBeforeRequest?.Invoke(new HttpRequestMessage(HttpMethod.Get, route));

            LogRequest(route, HttpMethod.Get);
            PopulateRequestHeaders(customHeaders);

            using (var response = await HttpClient.GetAsync(route, cancellationToken))
            {
                return await ProcessResponseAsync(response);
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponse<TResult>> GetAsync<TResult>(
            string route,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            var response = await GetAsync(route, customHeaders, cancellationToken);

            return new HttpResponse<TResult>(serializerOptions)
            {
                Content = response.Content,
                StatusCode = response.StatusCode
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponse> RequestAsync(
            HttpRequestMessage request,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            OnBeforeRequest?.Invoke(request);

            LogRequest(request.RequestUri.ToString(), request.Method);
            PopulateRequestHeaders(customHeaders);

            using (var response = await HttpClient.SendAsync(request, cancellationToken))
            {
                return await ProcessResponseAsync(response);
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponse<TResult>> RequestAsync<TResult>(
            HttpRequestMessage request,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            var response = await RequestAsync(request, customHeaders, cancellationToken);

            return new HttpResponse<TResult>(serializerOptions)
            {
                Content = response.Content,
                StatusCode = response.StatusCode
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponse> PostAsync(
            string route,
            object data = null,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            OnBeforeRequest?.Invoke(new HttpRequestMessage(HttpMethod.Post, route));

            LogRequest(route, HttpMethod.Post);
            PopulateRequestHeaders(customHeaders);

            var serialized = PrepareSerializedContent(data);

            using (var response = await HttpClient.PostAsync(route, serialized, cancellationToken))
            {
                return await ProcessResponseAsync(response);
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponse<TResult>> PostAsync<TResult>(
            string route,
            object data = null,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            var response = await PostAsync(route, data, customHeaders, cancellationToken);

            return new HttpResponse<TResult>(serializerOptions)
            {
                Content = response.Content,
                StatusCode = response.StatusCode
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponse> PutAsync(
            string route,
            object data = null,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            OnBeforeRequest?.Invoke(new HttpRequestMessage(HttpMethod.Put, route));

            LogRequest(route, HttpMethod.Put);
            PopulateRequestHeaders(customHeaders);

            var serialized = PrepareSerializedContent(data);

            using (var response = await HttpClient.PutAsync(route, serialized, cancellationToken))
            {
                return await ProcessResponseAsync(response);
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponse<TResult>> PutAsync<TResult>(
            string route,
            object data = null,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            var response = await PutAsync(route, data, customHeaders, cancellationToken);

            return new HttpResponse<TResult>(serializerOptions)
            {
                Content = response.Content,
                StatusCode = response.StatusCode
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponse> PatchAsync(
            string route,
            object data,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            OnBeforeRequest?.Invoke(new HttpRequestMessage(new HttpMethod("PATCH"), route));

            var httpMethod = new HttpMethod("PATCH");

            LogRequest(route, httpMethod);
            PopulateRequestHeaders(customHeaders);

            var serialized = PrepareSerializedContent(data);

            var request = new HttpRequestMessage(httpMethod, route)
            {
                Content = serialized
            };

            using (var response = await HttpClient.SendAsync(request, cancellationToken))
            {
                return await ProcessResponseAsync(response);
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponse<TResult>> PatchAsync<TResult>(
            string route,
            object data,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            var response = await PatchAsync(route, data, customHeaders, cancellationToken);

            return new HttpResponse<TResult>(serializerOptions)
            {
                Content = response.Content,
                StatusCode = response.StatusCode
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponse> DeleteAsync(
            string route,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            OnBeforeRequest?.Invoke(new HttpRequestMessage(HttpMethod.Delete, route));

            LogRequest(route, HttpMethod.Delete);
            PopulateRequestHeaders(customHeaders);

            using (var response = await HttpClient.DeleteAsync(route, cancellationToken))
            {
                return await ProcessResponseAsync(response);
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponse<TResult>> DeleteAsync<TResult>(
            string route,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            var response = await DeleteAsync(route, customHeaders, cancellationToken);

            return new HttpResponse<TResult>(serializerOptions)
            {
                Content = response.Content,
                StatusCode = response.StatusCode
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponse> DeleteAsync(
            string route,
            object data,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            OnBeforeRequest?.Invoke(new HttpRequestMessage(HttpMethod.Delete, route));

            LogRequest(route, HttpMethod.Delete);
            PopulateRequestHeaders(customHeaders);

            var serialized = PrepareSerializedContent(data);

            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, route)
            {
                Content = serialized
            };

            using (var response = await HttpClient.SendAsync(requestMessage, cancellationToken))
            {
                return await ProcessResponseAsync(response);
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponse<TResult>> DeleteAsync<TResult>(
            string route,
            object data,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default)
        {
            var response = await DeleteAsync(route, data, customHeaders, cancellationToken);

            return new HttpResponse<TResult>(serializerOptions)
            {
                Content = response.Content,
                StatusCode = response.StatusCode
            };
        }

        #region Internal data processing & logging

        protected virtual HttpContent PrepareSerializedContent(object data)
        {
            if (data is MultipartFormDataContent formDataContent)
            {
                return formDataContent;
            }

            var payload = data switch
            {
                string strData => strData,
                _ => JsonSerializer.Serialize(data, serializerOptions)
            };

            return new StringContent(payload, Encoding.UTF8, "application/json");
        }

        protected virtual void PopulateRequestHeaders(IDictionary<string, string> customHeaders = null)
        {
            if (customHeaders == null)
                return;

            foreach (var header in customHeaders)
            {
                // Ensure not to have duplicated headers
                if (HttpClient.DefaultRequestHeaders.Contains(header.Key))
                {
                    HttpClient.DefaultRequestHeaders.Remove(header.Key);
                }

                HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        protected virtual async Task<HttpResponse> ProcessResponseAsync(HttpResponseMessage response)
        {
            var result = new HttpResponse(serializerOptions)
            {
                StatusCode = response.StatusCode,
                Content = await response.Content.ReadAsStringAsync()
            };

            // Log it unsuccessful response
            if (!response.IsSuccessStatusCode && _logger != null)
            {
                var logMessage = $"Non-successful response returned from \"{response.RequestMessage.RequestUri}\" (HTTP code: {result.StatusCode}) - Response content: {result.Content}";
                _logger.LogWarning(logMessage);
            }

            return result;
        }

        protected virtual void LogRequest(string route, HttpMethod httpMethod)
        {
            if (_logger == null || Config == null)
                return;

            var uri = new Uri(new Uri(Config.Url), route);
            var logMessage = $"API calling: {httpMethod} {uri}";

            switch (Config.AuthType)
            {
                case AuthType.Anonymous:
                    logMessage += " (without authentication)";
                    break;
                case AuthType.Basic:
                    logMessage += $" (with username/clientId = \"{Config.ClientId}\")";
                    break;
                default:
                    logMessage += $" (using {Config.AuthType} authentication)";
                    break;
            }

            _logger.LogDebug(logMessage);
        }

        #endregion
    }
}
