using System.Text.Json.Serialization;

namespace GravityLib.Http.OAuth2
{
    /// <summary>
    /// OAuth token data model as by RFC 6749
    /// https://datatracker.ietf.org/doc/html/rfc6749
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// Usually it is "Bearer"
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "Bearer";

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// The lifetime in seconds of the access token
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// OPTIONAL. The refresh token, which can be used to obtain new access tokens using the same authorization grant
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// The lifetime in seconds of the refresh token
        /// </summary>
        [JsonPropertyName("refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }
    }
}
