using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Pricing.Services;
using SimplCommerce.Module.ShoppingCart.Services;
using SimplCommerce.Module.Checkouts.Services;
using SimplCommerce.Module.Catalog.Services;
using SimplCommerce.Module.ShippingPrices.Services;
using SimplCommerce.Module.Tax.Services;
using SimplCommerce.ServiceCommunication.Extensions;
using SimplCommerce.ServiceCommunication.Configuration;

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

// Add Identity (read-only for user validation)
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<SimplDbContext>()
    .AddDefaultTokenProviders();

// Add JWT Authentication (same settings as IdentityService)
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
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForJwtToken2024SimplCommerce!@#$%"))
    };
});

// Add Repository
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient(typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));

// Add Identity Service Client for inter-service communication
builder.Services.AddIdentityServiceClient(builder.Configuration);

// Add Order-related services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IProductPricingService, ProductPricingService>();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add Controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "SimplCommerce Order Service API", 
        Version = "v1",
        Description = "Microservice for Order Management and Product Purchase"
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

// Add CORS
builder.Services.AddCors(options =>
{
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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Service API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "OrderService", Timestamp = DateTime.UtcNow }));

app.Run();

