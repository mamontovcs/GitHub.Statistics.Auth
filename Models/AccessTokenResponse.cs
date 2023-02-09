using Newtonsoft.Json;

namespace GitHub.Statistics.Authentication.Models
{
    public class AccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
