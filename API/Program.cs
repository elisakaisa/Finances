using Common.Database;
using Common.Repositories;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // This section enables OAuth2 for Swagger
    options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect scope" },
                    { "email", "Email scope" },
                }
            }
        },
        In = ParameterLocation.Header,
        Description = "OAuth 2.0 authentication"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "OAuth2"
                }
            },
            new List<string> { "openid", "email" }
        }
    });
});

var dbConnectionString = builder.Configuration.GetConnectionString("DbConnection");
builder.Services.AddDbContext<FinancesDbContext>(options => 
    options.UseSqlServer(dbConnectionString));

// Register repositories
builder.Services.AddScoped<ICategoryRepository, CategorySubcategoryRepository>();
builder.Services.AddScoped<ISubcategoryRepository, CategorySubcategoryRepository>();
builder.Services.AddScoped<IMonthlyIncomeAfterTaxRepository, MonthlyIncomeAfterTaxRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHouseholdRepository, HouseholdRepository>();

// Register services
builder.Services.AddScoped<IMonthlyIncomeAfterTaxesService, MonthlyIncomeAfterTaxService>();
builder.Services.AddScoped<IRepartitionService, RepartitionService>();
builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwaggerUI",
        policy => policy.WithOrigins("https://localhost:7251") // Change to the port Swagger is running on
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

// Authentication (Google)
var configuration = builder.Configuration;
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    });


var app = builder.Build();

app.UseCors("AllowSwaggerUI");

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // This sets the OAuth 2.0 security definition in Swagger UI
        options.OAuthClientId(configuration["Authentication:Google:ClientId"]);
        options.OAuthClientSecret(configuration["Authentication:Google:ClientSecret"]);
        options.OAuthScopes("openid email");
        options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
    });
}

app.UseHttpsRedirection();


app.MapControllers();

app.Run();
