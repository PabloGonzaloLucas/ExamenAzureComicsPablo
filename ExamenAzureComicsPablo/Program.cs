using AyudaExamenViernes.Helpers;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add CORS support - read from configuration
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:4200", "https://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularLocal", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddTransient<HelperFotoTransform>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

// Ensure the Imagenes directory exists
string imagenesPath = Path.Combine(app.Environment.ContentRootPath, "Imagenes");
if (!Directory.Exists(imagenesPath))
{
    Directory.CreateDirectory(imagenesPath);
}

// Enable CORS middleware - MUST be before MapControllers
app.UseCors("AllowAngularLocal");

app.MapOpenApi(); 
app.MapScalarApiReference();


app.MapGet("/", context =>
{
    context.Response.Redirect("/scalar");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagenesPath),
    RequestPath = "/imagenes"
});

app.UseAuthorization();

app.MapControllers();

app.Run();
