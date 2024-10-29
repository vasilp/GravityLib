using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using GravityLib.Common.Exceptions;
using GravityLib.Http.Config;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GravityLib.Http.OAuth2;

public class OAuth2ApiClient : APIClient, IOAuth2ApiClient
{
    private readonly ILogger _logger;
    private readonly IMemoryCache _cache;
    private string _accessToken = "";
    private string _refreshToken = "";
    private DateTime _refreshTokenExpire = DateTime.MinValue;

    public OAuth2ApiClient(HttpClient client, IMemoryCache cache, APIClientConfig config, ILogger logger = null)
        : base(client, config, logger)
    {
        _cache = cache;
        _logger = logger;

        if (Config.AuthType == AuthType.OAuth2)
        {
            OnBeforeRequest += (request) => AcquireAccessToken();
        }
    }

    public OAuth2ApiClient(IMemoryCache cache, APIClientConfig config, ILogger logger = null)
        : base(config, logger)
    {
        _cache = cache;
        _logger = logger;

        if (Config.AuthType == AuthType.OAuth2)
        {
            OnBeforeRequest += (request) => AcquireAccessToken();
        }
    }

    /// <inheritdoc />
    public virtual async Task<HttpResponse<TokenResponse>> AcquireAccessTokenAsync(string clientId, string clientSecret, string scope = null, string route = "token", CancellationToken cancellationToken = default)
    {
        var data = new TokenRequest
        {
            GrantType = TokenRequest.GrantClientCredentials,
            Scope = scope,
            ClientId = clientId,
            ClientSecret = clientSecret
        };

        return await PostToOAuth2TokenAsync<TokenResponse>(route, data.ToDictionary());
    }

    /// <inheritdoc />
    public virtual async Task<HttpResponse<TokenResponse>> AcquireAccessTokenByRefreshTokenAsync(string clientId, string refreshToken, string route = "token", CancellationToken cancellationToken = default)
    {
        var data = new TokenRequest
        {
            GrantType = TokenRequest.GrantRefreshToken,
            ClientId = clientId,
            RefreshToken = refreshToken
        };

        return await PostToOAuth2TokenAsync<TokenResponse>(route, data.ToDictionary());
    }

    /// <inheritdoc />
    public virtual async Task<HttpResponse> RevokeTokenAsync(string clientId, string token, string route = "token/revoke", CancellationToken cancellationToken = default)
    {
        var data = new RevokeTokenRequest
        {
            ClientId = clientId,
            Token = token
        };

        return await PostToOAuth2TokenAsync<TokenResponse>(route, data.ToDictionary());
    }

    /// <summary>
    /// Makes a request to the OAuth2 "token" endpoint with the provided data.
    /// </summary>
    protected virtual async Task<HttpResponse<TResult>> PostToOAuth2TokenAsync<TResult>(string route, IDictionary<string, string> data)
    {
        var content = Config.OAuth2UseJson
            ? PrepareSerializedContent(data)   // Use JSON format
            : new FormUrlEncodedContent(data); // Use RFC standard and "application/x-www-form-urlencoded" format

        LogRequest(route, HttpMethod.Post);

        using (var responseMessage = await HttpClient.PostAsync(route, content))
        {
            var response = await ProcessResponseAsync(responseMessage);

            return new HttpResponse<TResult>(serializerOptions)
            {
                Content = response.Content,
                StatusCode = response.StatusCode
            };
        }
    }

    public void AcquireAccessToken()
    {
        // TODO: Use semaphore with WaitAsync() to avoid blocking requests for token
        lock (_accessToken)
        {
            _accessToken = _cache.GetOrCreate(GetType() + ".AccessToken", (entry) =>
            {
                // Note: Cannot have async code in a lock section

                // Request new access token by refresh token (defaults to unauthorized request response if not obtained still)
                var result = new HttpResponse<TokenResponse>()
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };

                if (_refreshTokenExpire > DateTime.UtcNow && !string.IsNullOrWhiteSpace(_refreshToken))
                {
                    result = Task.Run(() => AcquireAccessTokenByRefreshTokenAsync(Config.ClientId, _refreshToken, Config.OAuth2TokenEndpoint, CancellationToken.None)).Result;
                }

                if (!result.IsSuccessStatusCode)
                {
                    // Request new access token by Client Id and Secret if cannot be done with refresh token
                    result = Task.Run(() => AcquireAccessTokenAsync(Config.ClientId, Config.ClientSecret, Config.OAuth2TokenScope, Config.OAuth2TokenEndpoint, CancellationToken.None)).Result;

                    if (!result.IsSuccessStatusCode)
                    {
                        string description;
                        try
                        {
                            description = result.Parse<TokenErrorResponse>()?.ErrorDescription;
                        }
                        catch
                        {
                            description = result.Content;
                        }

                        _logger.LogCritical($"Cannot obtain access token for \"{Config.Url}\". Error: {description}");

                        throw new RemoteException(description, result.StatusCode);
                    }
                }

                var tokenResponse = result.Data;

                _logger.LogDebug($"Access token obtained for \"{Config.Url}\"");

                _refreshToken = tokenResponse.RefreshToken;
                _refreshTokenExpire = DateTime.Now.AddSeconds(tokenResponse.RefreshTokenExpiresIn - 60); // Subtract some seconds just in case if the machines have some slight time diff

                entry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(tokenResponse.ExpiresIn - 60); // Subtract some seconds just in case if the machines have some slight time diff
                return result.Data.AccessToken ?? "";
            });

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
    }
}
