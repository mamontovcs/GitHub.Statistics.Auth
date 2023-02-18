using GitHub.Statistics.Authentication.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
var clientPolicyName = "ClientPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(clientPolicyName, builder => builder
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.UseCors(clientPolicyName);

app.MapHub<AuthenticationHub>("/authenticationHub");

app.Run();