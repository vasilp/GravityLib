using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GravityLib.Common.Exceptions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Fallback;
using Polly.Retry;

namespace GravityLib.Http.HttpRetry
{
    public class HttpRetryMessageHandler : DelegatingHandler
    {
        private const string IsTransientErrorKey = "IsTransientErrorKey";
        private readonly int _retryCount = 5;
        private readonly int _retryInterval = 1000;
        private readonly ILogger _logger;

        public HttpRetryMessageHandler(ILogger logger, int retryCount, int retryInterval)
            : this(retryCount, retryInterval)
        {
            _logger = logger;
        }

        public HttpRetryMessageHandler(int retryCount, int retryInterval)
        {
            _retryCount = retryCount;
            _retryInterval = retryInterval;
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            InnerHandler = handler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var builder = new StringBuilder();

            var transientRetryPolicy = GetTransientRetryPolicy(builder);

            return await transientRetryPolicy
                .WrapAsync(GetTransientFallbackPolicy(builder))
                .ExecuteAsync(() => base.SendAsync(request, cancellationToken));
        }

        public AsyncRetryPolicy GetTransientRetryPolicy(StringBuilder builder)
        {
            var transientRetryPolicy = Policy
                .Handle<RemoteException>(ex => ex.IsTransient)
                .WaitAndRetryAsync(_retryCount, retryAttempt =>
                {
                    // TODO: Figure out a way to do a one only retry for message found policy
                    builder.AppendLine($"Retry attempt: {retryAttempt} of {_retryCount}");

                    return TimeSpan.FromMilliseconds(_retryInterval);
                }, (ex, retryTimeSpan, context) =>
                {
                    builder.AppendLine($"Retrying interval in milliseconds: {retryTimeSpan}.");

                    _logger.LogInformation(builder.ToString());
                    builder.Clear();
                });

            return transientRetryPolicy;
        }

        public AsyncFallbackPolicy<HttpResponseMessage> GetTransientFallbackPolicy(StringBuilder builder)
        {
            var transientFallbackPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>(httpRequestException =>
                {
                    return ExceptionHelper.IsTransient(httpRequestException.Message);
                })
                .OrInner<WebException>(webException =>
                {
                    return ExceptionHelper.IsTransient(webException.Status, webException.Message);
                })
                .OrResult(response =>
                {
                    return !response.IsSuccessStatusCode;
                })
                .FallbackAsync(async (delegateOutcome, context, token) =>
                {
                    // This is the onFallback action. We enter this part when the request has failed and after the onFallback    function. Here we decide what to do with the request. We throw exception which we use in the actual retry     policy if the error is transient. Else we return the response as normal.
                    if (bool.TryParse(context[IsTransientErrorKey].ToString(), out bool isTransient) && isTransient)
                    {
                        var responseContent = await delegateOutcome.Result.Content.ReadAsStringAsync();

                        throw new RemoteException(
                            $"Http request failed with transient error. Response: {(string.IsNullOrWhiteSpace(responseContent) ? "No response found" : responseContent)}",
                            delegateOutcome.Result,
                            isTransient);
                    }

                    return delegateOutcome.Result;
                }, async (delegateOutcome, context) =>
                {
                    // This is the onFallback function. We enter this part when the request fails.
                    var isTransient = false;

                    if (delegateOutcome.Result != null)
                    {
                        var responseContent = await delegateOutcome.Result.Content.ReadAsStringAsync();
                        isTransient = ExceptionHelper.IsTransient(delegateOutcome.Result.StatusCode, responseContent);

                        if (isTransient)
                        {
                            builder.AppendLine($"Http request failed." +
                                $" Status code was not successful for request with url:     {delegateOutcome.Result.RequestMessage.RequestUri}" +
                                $" and method type: {delegateOutcome.Result.RequestMessage.Method}." +
                                $" Transient error detected." +
                                $" Http response for failed request: {(string.IsNullOrWhiteSpace(responseContent) ? "No response found" : responseContent)}");
                        }
                    }
                    else if (delegateOutcome.Exception != null)
                    {
                        isTransient = ExceptionHelper.IsTransient(delegateOutcome.Exception.Message);

                        builder.AppendLine($"Http request failed. " +
                            $"Exception occurred. Transient error detected." +
                            $" Exception message: {(string.IsNullOrWhiteSpace(delegateOutcome.Exception.Message) ? "Empty exception message." : delegateOutcome.Exception.Message)}." +
                            $" Exception stack trace: { (string.IsNullOrWhiteSpace(delegateOutcome.Exception.StackTrace) ? "Empty   exception stack trace" : delegateOutcome.Exception.StackTrace)}");
                    }

                    if (context.ContainsKey(IsTransientErrorKey))
                    {
                        context[IsTransientErrorKey] = isTransient;
                    }
                    else
                    {
                        context.Add(IsTransientErrorKey, isTransient);
                    }
                });

            return transientFallbackPolicy;
        }
    }
}
