using CampusEats.API.Extensions;
using CampusEats.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = false;
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CampusEats API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "CampusEats API Documentation";
        c.DefaultModelsExpandDepth(-1); // Hide schemas section
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowedOrigins");

// Add custom error handling
app.UseErrorHandling();

// Add authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// Add JWT middleware
app.UseJwtMiddleware();

app.MapControllers();

app.Run();