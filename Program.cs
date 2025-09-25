using DotNetEnv;
using jitsi_oauth.Extensions;
using jitsi_oauth.Middlewares;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAppServices();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors/{0}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

// app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
