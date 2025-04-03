using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StocksApi;
using StocksApi.DatabaseContext;
using StocksApi.IRepositoryContracts;
using StocksApi.IServiceContracts;
using StocksApi.IServiceContracts.Finnhub;
using StocksApi.Middleware;
using StocksApi.Repositories;
using StocksApi.ServiceContracts;
using StocksApi.Services;
using StocksApi.Services.Finnhub;
using StocksApi.Bgs;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Load configuration
var configuration = builder.Configuration;
//this service by itself when application starts running and updates stock data inside redis cahce  so we need to wait for client side
builder.Services.AddSingleton<StockDataRefresher>();  // ✅ Register as singleton
builder.Services.AddHostedService(provider => provider.GetRequiredService<StockDataRefresher>()); // ✅ Ensure proper initialization


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379"));


// 🔹 Configure trading options
builder.Services.Configure<TradingOptions>(configuration.GetSection("TradingOptions"));
builder.Services.AddTransient<IJwtService, JwtService>();
// 🔹 Add Controllers with JSON format support
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new ProducesAttribute("application/json"));
    opt.Filters.Add(new ConsumesAttribute("application/json"));
}).AddXmlSerializerFormatters();

// 🔹 Identity & Authentication
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
{
    opt.Password.RequiredLength = 5;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireDigit = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
.AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        ValidateIssuer = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});

// 🔹 Add Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// 🔹 Register Repositories & Services (Using Scoped for better performance)
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IBuyOrderService, StockBuyOrderServices>();
builder.Services.AddScoped<ISellOrderService, StockSellOrderServices>();
builder.Services.AddScoped<IFinnhubRepository, FinnhubRepository>();
builder.Services.AddScoped<IFinnhubCompanyProfileService, FinnhubCompanyProfileService>();
builder.Services.AddScoped<IFinnhubSearchStockService, FinnhubSearchStockService>();
builder.Services.AddScoped<IFinnhubStockPriceQuoteService, FinnhubStockPriceQuote>();
builder.Services.AddScoped<IFinnhubStockService, FinnhubStockServcie>(); // 🔹 Fixed Typo
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddHttpClient<IFinnhubRepository, FinnhubRepository>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(180); // 🔹 Increase timeout to 3 minutes
});

// 🔹 Optimize HttpClient Usage
builder.Services.AddHttpClient<IFinnhubRepository, FinnhubRepository>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5)); // 🔹 Prevents socket exhaustion

// 🔹 Register Middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// 🔹 Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
    {
        policyBuilder.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>())
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});

// 🔹 Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowSpecificOrigins"); // 🔹 Use Named CORS Policy
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
