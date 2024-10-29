using System.Text.Json.Serialization;

namespace GravityLib.Http.OAuth2
{
    public class TokenErrorResponse
    {
        /// <summary>
        /// Response error
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }

        /// <summary>
        /// Error description
        /// </summary>
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
    }
}
