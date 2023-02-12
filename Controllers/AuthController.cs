using GitHub.Statistics.Authentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GitHub.Statistics.Authentication.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        [HttpGet("/login")]
        public async Task Login()
        {
            await HttpContext.ChallengeAsync("GitHub", new AuthenticationProperties() { RedirectUri = "/" });
        }

        [HttpGet("/signin-github-token")]
        public async Task ExchangeCode([FromQuery(Name = "code")] string code)
        {
            Console.WriteLine("Code: " + code);
            var options = HttpContext.RequestServices.
                GetRequiredService<IOptionsMonitor<OAuthOptions>>().Get("GitHub");

            var pairs = new Dictionary<string, string>()
                    {
                            { "client_id", options.ClientId },
                            { "client_secret", options.ClientSecret },
                            { "code", code },
                            { "redirect_uri", "" }
                    };
            var content = new FormUrlEncodedContent(pairs);
            var refreshResponse = await options.Backchannel.PostAsync(options.TokenEndpoint, content, HttpContext.RequestAborted);
            refreshResponse.EnsureSuccessStatusCode();

            var payload = await refreshResponse.Content.ReadAsStringAsync();

            var accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(payload);

            var httpclient = new HttpClient();
            var message = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"http://githubstatistics-web-1:80/test/setToken?token={accessTokenResponse.AccessToken}", UriKind.Absolute)
            };

            Console.WriteLine("Token:" + accessTokenResponse.AccessToken);
            await httpclient.SendAsync(message);
        }
    }
}
