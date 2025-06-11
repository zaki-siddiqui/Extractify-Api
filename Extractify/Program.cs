using Microsoft.EntityFrameworkCore;
using Extractify.Infrastructure.Data;
using Extractify.Infrastructure.Repositories;
using Extractify.Application.Services;
using Extractify.Application.Interfaces;
using Extractify.Infrastructure.Scraping;
using AutoMapper;
using Extractify.Application.Mappings;
using Extractify.Api.Middleware;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Extractify.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console()
          .WriteTo.File("logs/extractify.log", rollingInterval: RollingInterval.Day)
          .MinimumLevel.Debug();
});

// Add logging
builder.Services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Information));

// Add services
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IScrapingService, ScrapingService>();
builder.Services.AddHttpClient(); // Registers IHttpClientFactory
builder.Services.AddScoped<IScraperClient, ScraperClient>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Validate DI
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Validating DI for IScraperClient");
        scope.ServiceProvider.GetRequiredService<IScraperClient>();
        logger.LogInformation("Validating DI for IScrapingService");
        scope.ServiceProvider.GetRequiredService<IScrapingService>();
        logger.LogInformation("DI validation successful");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "DI Validation Error");
        throw;
    }
}


var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();


//using Microsoft.EntityFrameworkCore;
//using Extractify.Infrastructure.Data; 
//using Extractify.Infrastructure.Scraping; 
//using Extractify.Application.Services; 
//using Extractify.Application.Interfaces; 
//using Extractify.Domain.Interfaces; 
//using Extractify.Infrastructure.Repositories; 
//using AutoMapper; 
//using Serilog; 
//using Microsoft.AspNetCore.Authentication.JwtBearer; 
//using Microsoft.IdentityModel.Tokens; 
//using System.Text;
//using Extractify.Application.Mappings;

//var builder = WebApplication.CreateBuilder(args);

//// Add Serilog
//Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger(); 

//builder.Host.UseSerilog();

//// Add services to the container.

//builder.Services.AddControllers().AddNewtonsoftJson(
//    options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
//    );
////builder.Services.AddControllers();


//// Add EF Core
//builder.Services.AddDbContext<ApplicationDbContext>(
//    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//// Add HttpClient for scraping
//builder.Services.AddHttpClient<ScraperClient>(client =>
//{
//    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
//});


//// Add AutoMapper
////builder.Services.AddAutoMapper(typeof(Program));
//builder.Services.AddAutoMapper(typeof(MappingProfile));

//// Add services and repositories
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IScrapingService, ScrapingService>();


//// Add JWT Authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//    };
//});


//// Configure CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy.WithOrigins("http://localhost:4200")
//        .AllowAnyMethod() // Allows GET, POST, OPTIONS, etc.
//        .AllowAnyHeader() // Allows Content-Type, Authorization, etc.
//        .AllowCredentials(); // If using authentication
//    });
//});

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//// Apply CORS policy before routing
//app.UseCors("AllowFrontend");

//app.UseAuthorization();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
