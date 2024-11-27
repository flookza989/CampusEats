using CampusEats.Web.Configuration;
using CampusEats.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddWebServices(builder.Configuration);

var app = builder.Build();

// Configure the application
app.ConfigureWebApplication();

// Add static files with cache busting
var cachePeriod = app.Environment.IsDevelopment() ? "0" : "604800";
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
            "Cache-Control", $"public, max-age={cachePeriod}");
    }
});


app.Run();