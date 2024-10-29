using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GravityLib.Http.Config;

namespace GravityLib.Http
{
    public interface IAPIClient
    {
        /// <summary>
        /// Web client configuration such as url, authentication type etc.
        /// </summary>
        APIClientConfig Config { get; }

        /// <summary>
        /// Http client instance
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Loads a configuration for the client 
        /// </summary>
        /// <param name="config"> The WebApi client configuration </param>
        void Configure(APIClientConfig config);

        /// <summary>
        /// Calls a WebApi using GET 
        /// </summary>
        /// <param name="route">The route that we want to call</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        /// <returns>Data from the WebApi. The method will bubble up the exception if it is raised</returns>
        Task<HttpResponse> GetAsync(
            string route,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using GET 
        /// </summary>
        /// <typeparam name="TResult">The type of the data the WebApi will return</typeparam>
        /// <param name="route">The route that we want to call</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        /// <returns>Data from the WebApi. The method will bubble up the exception if it is raised</returns>
        Task<HttpResponse<TResult>> GetAsync<TResult>(
            string route,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using custom HttpRequestMessage content 
        /// </summary>
        /// <param name="request">The route that we want to call</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        Task<HttpResponse> RequestAsync(
            HttpRequestMessage request,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using custom HttpRequestMessage content 
        /// </summary>
        /// <typeparam name="TResult">The type of the data the WebApi will return</typeparam>
        /// <param name="request">The route that we want to call</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        Task<HttpResponse<TResult>> RequestAsync<TResult>(
            HttpRequestMessage request,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using POST 
        /// </summary>
        /// <param name="route">The route that we want to call</param>
        /// <param name="data">The object to be sent. You can use <see cref="MultipartFormDataContent"/> as well.</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        Task<HttpResponse> PostAsync(
            string route,
            object data = null,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using POST 
        /// </summary>
        /// <typeparam name="TResult">The type of the data the WebApi will return</typeparam>
        /// <param name="route">The route that we want to call</param>
        /// <param name="data">The object to be sent. You can use <see cref="MultipartFormDataContent"/> as well.</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        Task<HttpResponse<TResult>> PostAsync<TResult>(
            string route,
            object data = null,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using PUT and the provided headers
        /// </summary>
        /// <param name="route">The route that we want to call</param>
        /// <param name="data">The object to be sent. You can use <see cref="MultipartFormDataContent"/> as well.</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        /// <returns>Data from the WebApi. The method will bubble up the exception if it is raised</returns>
        Task<HttpResponse> PutAsync(
            string route,
            object data = null,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using PUT and the provided headers
        /// </summary>
        /// <typeparam name="TResult">The type of the data the WebApi will return</typeparam>
        /// <param name="route">The route that we want to call</param>
        /// <param name="data">The object to be sent. You can use <see cref="MultipartFormDataContent"/> as well.</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        /// <returns>Data from the WebApi. The method will bubble up the exception if it is raised</returns>
        Task<HttpResponse<TResult>> PutAsync<TResult>(
            string route,
            object data = null,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using PATCH and the provided headers
        /// </summary>
        /// <param name="route">The route that we want to call</param>
        /// <param name="data">The object to be sent. You can use <see cref="MultipartFormDataContent"/> as well.</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        /// <returns>Data from the WebApi. The method will bubble up the exception if it is raised</returns>
        Task<HttpResponse> PatchAsync(
            string route,
            object data,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using PATCH and the provided headers
        /// </summary>
        /// <typeparam name="TResult">The type of the data the WebApi will return</typeparam>
        /// <param name="route">The route that we want to call</param>
        /// <param name="data">The object to be sent. You can use <see cref="MultipartFormDataContent"/> as well.</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        /// <returns>Data from the WebApi. The method will bubble up the exception if it is raised</returns>
        Task<HttpResponse<TResult>> PatchAsync<TResult>(
            string route,
            object data,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using DELETE and the provided headers
        /// </summary>
        /// <param name="route">The route that we want to call</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        /// <returns>Data from the WebApi. The method will bubble up the exception if it is raised</returns>
        Task<HttpResponse> DeleteAsync(
            string route,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using DELETE and the provided headers
        /// </summary>
        /// <typeparam name="TResult">The type of the data the WebApi will return</typeparam>
        /// <param name="route">The route that we want to call</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        /// <returns>Data from the WebApi. The method will bubble up the exception if it is raised</returns>
        Task<HttpResponse<TResult>> DeleteAsync<TResult>(
            string route,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using DELETE method. Use with precaution as Layer7 doesn't work well with Delete + Body
        /// </summary>
        /// <param name="route">The route that we want to call</param>
        /// <param name="data">The object to be sent. You can use <see cref="MultipartFormDataContent"/> as well.</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        /// <returns>Data from the WebApi. The method will bubble up the exception if it is raised</returns>
        Task<HttpResponse> DeleteAsync(
            string route,
            object data,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calls a WebApi using DELETE method. Use with precaution as Layer7 doesn't work well with Delete + Body
        /// </summary>
        /// <typeparam name="T">The type of the data the WebApi will return</typeparam>
        /// <param name="route">The route that we want to call</param>
        /// <param name="data">The object to be sent. You can use <see cref="MultipartFormDataContent"/> as well.</param>
        /// <param name="customHeaders">A collection of headers to be applied to the request</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This token can be used to cancel the operation before it completes.</param>
        /// <returns>Data from the WebApi. The method will bubble up the exception if it is raised</returns>
        Task<HttpResponse<TResult>> DeleteAsync<TResult>(
            string route,
            object data,
            IDictionary<string, string> customHeaders = null,
            CancellationToken cancellationToken = default);
    }
}
