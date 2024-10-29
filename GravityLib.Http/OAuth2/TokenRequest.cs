using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace GravityLib.Http.OAuth2
{
    /// <summary>
    /// OAuth token request data model as by RFC 6749
    /// https://datatracker.ietf.org/doc/html/rfc6749
    /// https://auth0.com/docs/api/authentication?http#authorization-code-flow44
    /// https://auth0.com/docs/get-started/applications/application-grant-types#available-grant-types
    /// </summary>
    public class TokenRequest : IValidatableObject
    {
        public const string GrantAuthorizationCode = "authorization_code";
        public const string GrantClientCredentials = "client_credentials";
        public const string GrantRefreshToken = "refresh_token";

        public static string[] ValidGrants = new[] { GrantAuthorizationCode, GrantClientCredentials, GrantRefreshToken };

        /// <summary>
        /// Denotes the flow you are using. Supported value is "authorization_code" and "refresh_token" based on the desired action.
        /// </summary>
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; }

        /// <summary>
        /// The scope of the access request. Optional.
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        /// <summary>
        /// Your application's Client ID.
        /// </summary>
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Your application's Client Secret.
        /// </summary>
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }
        
        /// <summary>
        /// The Refresh Token to use to obtain new access tokens using the same authorization grant
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!ValidGrants.Contains(GrantType))
            {
                yield return new ValidationResult($"Invalid grant type: {GrantType}", new[] { nameof(GrantType) });
            }

            if ((GrantType is GrantAuthorizationCode or GrantClientCredentials) && (string.IsNullOrWhiteSpace(ClientId) || string.IsNullOrWhiteSpace(ClientSecret)))
            {
                yield return new ValidationResult($"{nameof(ClientId)} and {nameof(ClientSecret)} are required for grant type: {GrantType}", new[] { nameof(ClientId), nameof(ClientSecret) });
            }

            if (GrantType == GrantRefreshToken && string.IsNullOrWhiteSpace(RefreshToken))
            {
                yield return new ValidationResult($"{nameof(RefreshToken)} is required for grant type: {GrantType}", new[] { nameof(RefreshToken) });
            }
        }

        public static TokenRequest FromFormDictionary(IReadOnlyDictionary<string, string> formData)
        {
            return new TokenRequest
            {
                GrantType = formData.TryGetValue("grant_type", out var val1) ? val1 : null,
                Scope = formData.TryGetValue("scope", out var val2) ? val2 : null,
                ClientId = formData.TryGetValue("client_id", out var val3) ? val3 : null,
                ClientSecret = formData.TryGetValue("client_secret", out var val4) ? val4 : null,
                RefreshToken = formData.TryGetValue("refresh_token", out var val5) ? val5 : null
            };
        }

        public IDictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { "grant_type", GrantType },
                { "scope", Scope },
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "refresh_token", RefreshToken }
            };
        }
    }
}
