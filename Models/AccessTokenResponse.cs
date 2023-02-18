using System.Text.Json.Serialization;

namespace GitHub.Statistics.Authentication.Models
{
    public class AccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
