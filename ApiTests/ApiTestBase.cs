using ApiTests.Helpers;
using AutoFixture;
using Common.Database;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Testcontainers.MsSql;

namespace ApiTests
{
    [Category("ApiTests")]
    public class ApiTestBase
    {
        private const ushort MsSqlPort = 1433;

        private WebApplicationFactory<Program> _factory;
        private IContainer _container;
        protected HttpClient HttpClient;
        protected readonly Fixture Fixture = new();

        protected static readonly JsonSerializerOptions JsonSerilaizerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            SetUpTestContainerSqlServer();
            await _container.StartAsync();

            ConfigureDatabase();
            await InitializeDatabase();

            HttpClient = _factory.CreateClient();

            ConfigureAutoFixtureBehavior();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
            HttpClient.Dispose();
            await _factory.DisposeAsync();
        }

        protected async Task ExecuteScopedContextAction(Action<FinancesDbContext> contextAction, bool saveChanges = true)
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinancesDbContext>();
            contextAction(dbContext);
            if (saveChanges) await dbContext.SaveChangesAsync();
        }

        private void SetUpTestContainerSqlServer()
        {
            _container = new ContainerBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPortBinding(MsSqlPort, true)
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("SA_PASSWORD", MsSqlBuilder.DefaultPassword)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlPort))
                .Build();
        }

        private void ConfigureDatabase()
        {
            var host = _container.Hostname;
            var port = _container.GetMappedPublicPort(MsSqlPort);

            var connectionString = $"server={host},{port};user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database={MsSqlBuilder.DefaultDatabase};TrustServerCertificate=True";
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddDbContext<FinancesDbContext>(options =>
                            options.UseSqlServer(connectionString));
                    });
                });
        }

        private async Task InitializeDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinancesDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        private void ConfigureAutoFixtureBehavior()
        {
            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            Fixture.Customizations.Add(new UserShareSpecimenBuilder());
            Fixture.Customizations.Add(new DateOnlySpecimenBuilder());
        }
    }
}