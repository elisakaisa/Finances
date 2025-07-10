using API.Middleware;
using Common.Database;
using Common.Repositories;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }