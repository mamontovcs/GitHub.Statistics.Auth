using GitHub.Statistics.Authentication.Models;
using GitHub.Statistics.Authentication.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace GitHub.Statistics.Authentication.Controllers
{
    /// <summary>
    /// Controller, which is responsible for user authentication
    /// </summary>
    [Route("[controller]")]
    public class AuthtenticationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHubContext<AuthenticationHub> _hubContext;

        /// <summary>
        /// Creates instance of <see cref="AuthtenticationController"/>
        /// </summary>
        /// <param name="hubContext">SignalR hub context</param>
        public AuthtenticationController(IHttpClientFactory httpClientFactory, IHubContext<AuthenticationHub> hubContext)
        {
            _httpClientFactory = httpClientFactory;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Provides authentication url
        /// </summary>
        /// <returns>Authentication url</returns>
        [HttpGet("getOAuthLink")]
        public IActionResult GetOAuthLink()
        {
            var url = "https://github.com/login/oauth/authorize?" +
                "client_id=c4b863f609b87f469301&redirect_uri=http://localhost:2509/authtentication/signin-github-token";
            var urlJson = JsonSerializer.Serialize(url);

            return Ok(JsonSerializer.Serialize(urlJson));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("signin-github-token")]
        public async Task<IActionResult> ExchangeCode([FromQuery(Name = "code")] string code)
        {
            var httpclient = _httpClientFactory.CreateClient();
            httpclient.DefaultRequestHeaders.Add("Accept", "application/json"); // Change

            Console.WriteLine("Code: " + code);

            var pairs = new Dictionary<string, string>()
                    {
                            { "client_id", "c4b863f609b87f469301" },
                            { "client_secret", "01d13e48ccb87bba4ef5f62f4aef401cf700ecbe" },
                            { "code", code },
                            { "redirect_uri", "" }
                    };
            var content = new FormUrlEncodedContent(pairs);
            var refreshResponse = await httpclient.PostAsync("https://github.com/login/oauth/access_token", content, HttpContext.RequestAborted);
            refreshResponse.EnsureSuccessStatusCode();

            var payload = await refreshResponse.Content.ReadAsStringAsync();

            var accessTokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(payload);

            await _hubContext.Clients.All.SendAsync("SendAccessTokenToClient", accessTokenResponse.AccessToken);

            return Ok("success");
        }
    }
}
