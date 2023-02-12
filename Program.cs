var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication()
    .AddOAuth("GitHub", "GitHub AccessToken only", o =>
    {
        o.ClientId = "c4b863f609b87f469301";
        o.ClientSecret = "01d13e48ccb87bba4ef5f62f4aef401cf700ecbe";
        o.CallbackPath = new PathString("/signin-github-token");
        o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        o.TokenEndpoint = "https://github.com/login/oauth/access_token";
        o.SaveTokens = true;
        o.Backchannel = new HttpClient();
        o.Backchannel.DefaultRequestHeaders.Add("Accept", "application/json");
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();