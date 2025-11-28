using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.IdentityService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Set Global Configuration
GlobalConfiguration.WebRootPath = builder.Environment.WebRootPath;
GlobalConfiguration.ContentRootPath = builder.Environment.ContentRootPath;

// Add DbContext
builder.Services.AddDbContext<SimplDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity
builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<SimplDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForJwtToken2024!@#"))
    };
});

// Add Services
builder.Services.AddScoped<ITokenService, TokenService>();

// Add Controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "SimplCommerce Identity Service API", 
        Version = "v1",
        Description = "Microservice for User Registration, Authentication and User Management"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add CORS for inter-service communication
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrderService", policy =>
    {
        policy.WithOrigins(
            builder.Configuration["ServiceUrls:OrderService"] ?? "https://localhost:5002",
            "https://localhost:5002",
            "http://localhost:5002"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
    
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API V1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "IdentityService", Timestamp = DateTime.UtcNow }));

app.Run();

