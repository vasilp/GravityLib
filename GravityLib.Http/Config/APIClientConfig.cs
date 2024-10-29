namespace GravityLib.Http.Config;

/// <summary>
/// A WebApi client configuration
/// </summary>
public class APIClientConfig
{
    /// <summary>
    /// The root URL that points to the remote server
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Defines the type authentication in the requests
    /// </summary>
    public AuthType AuthType { get; set; }

    /// <summary>
    /// Defines if the /token endpoint of the OAuth2 request should use JSON content type instead of "form-urlencoded"
    /// </summary>
    public bool OAuth2UseJson { get; set; }

    /// <summary>
    /// The scope of the OAuth2 access request. Optional.
    /// </summary>
    public string OAuth2TokenScope { get; set; }

    /// <summary>
    /// The endpoint that will be used to obtain the OAuth2 Bearer token.
    /// Do not put "/" in the beginning of the string, as it will be treated as an absolute path to the hostname only.
    /// </summary>
    public string OAuth2TokenEndpoint { get; set; } = "token";

    /// <summary>
    /// The endpoint that will be used to revoke an OAuth2 Bearer token.
    /// Do not put "/" in the beginning of the string, as it will be treated as an absolute path to the hostname only.
    /// </summary>
    public string OAuth2TokenRevokeEndpoint { get; set; } = "token/revoke";

    /// <summary>
    /// The username / clientId that will be used in requests
    /// when using Basic authentication or obtaining Bearer tokens
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// The user password / client secret that will be used in requests
    /// when using Basic authentication or obtaining Bearer tokens
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    /// The Token / Bearer token value that will be used in requests
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Indicates how many times the request should be retried
    /// </summary>
    public int RetryCount { get; set; } = 7;

    /// <summary>
    /// Retry interval in milliseconds which is used after each retry attempt
    /// </summary>
    public int RetryInterval { get; set; } = 3000;
}