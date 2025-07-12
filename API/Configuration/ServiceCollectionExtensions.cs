using Common.Database;
using Common.Repositories.Interfaces;
using Common.Repositories;
using Common.Services.Interfaces;
using Common.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnectionString = configuration.GetConnectionString("DbConnection");
            services.AddDbContext<FinancesDbContext>(options =>
                options.UseSqlServer(dbConnectionString));
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategorySubcategoryRepository>();
            services.AddScoped<ISubcategoryRepository, CategorySubcategoryRepository>();
            services.AddScoped<IMonthlyIncomeAfterTaxRepository, MonthlyIncomeAfterTaxRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHouseholdRepository, HouseholdRepository>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IMonthlyIncomeAfterTaxesService, MonthlyIncomeAfterTaxService>();
            services.AddScoped<IRepartitionService, RepartitionService>();
            services.AddScoped<ISummaryService, SummaryService>();
            services.AddScoped<ITransactionService, TransactionService>();
            return services;
        }

        public static IServiceCollection AddControllersWithCustomJsonOptions(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
            return services;
        }
    }
}
