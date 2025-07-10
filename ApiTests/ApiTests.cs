using Common.Database;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTests
{
    [TestFixture]
    public class ApiTests
    {
        private const string Database = "master";
        private const string Username = "sa";
        private const string Password = "$trongPassword";
        private const ushort MsSqlPort = 1433;

        private WebApplicationFactory<Program> _factory;
        private IContainer _container;
        protected HttpClient HttpClient;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Set up Testcontainers SQL Server container
            _container = new ContainerBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPortBinding(MsSqlPort, true)
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("SQLCMDUSER", Username)
                .WithEnvironment("SQLCMDPASSWORD", Password)
                .WithEnvironment("MSSQL_SA_PASSWORD", Password)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlPort))
                .Build();

            //Start Container
            await _container.StartAsync();

            var host = _container.Hostname;
            var port = _container.GetMappedPublicPort(MsSqlPort);

            // Replace connection string in DbContext
            var connectionString = $"Server={host},{port};Database={Database};User Id={Username};Password={Password};TrustServerCertificate=True";
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddDbContext<FinancesDbContext>(options =>
                            options.UseSqlServer(connectionString));
                    });
                });

            // Initialize database
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinancesDbContext>();
            dbContext.Database.Migrate();

            HttpClient = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
            _factory.Dispose();
        }

        protected async Task ExecuteScopedContextAction(Action<FinancesDbContext> contextAction, bool saveChanges = true)
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinancesDbContext>();
            contextAction(dbContext);
            if (saveChanges) await dbContext.SaveChangesAsync();
        }
    }
}