using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GravityLib.Http.OAuth2;

/// <summary>
/// OAuth revoke token request data model as by RFC 6749
/// https://auth0.com/docs/api/authentication?http#revoke-refresh-token
/// </summary>
public class RevokeTokenRequest
{
    /// <summary>
    /// Your application's Client ID.
    /// </summary>
    [JsonPropertyName("client_id")]
    [Required]
    public string ClientId { get; set; }
        
    /// <summary>
    /// The token to be revoked
    /// </summary>
    [JsonPropertyName("token")]
    [Required]
    public string Token { get; set; }

    public static RevokeTokenRequest FromFormDictionary(IReadOnlyDictionary<string, string> formData)
    {
        return new RevokeTokenRequest
        {
            ClientId = formData.TryGetValue("client_id", out var val1) ? val1 : null,
            Token = formData.TryGetValue("token", out var val2) ? val2 : null
        };
    }

    public IDictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "client_id", ClientId },
            { "token", Token }
        };
    }
}