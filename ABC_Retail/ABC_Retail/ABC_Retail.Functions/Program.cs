using ABC_Retail.Data;
using ABC_Retail.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Get connection strings from environment variables
        var tableStorageConnection = Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;
        var sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString")!;

        // Register TableStorage as singleton with connection string
        services.AddSingleton(new TableStorage(tableStorageConnection));

        // Register DbContext for SQL
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(sqlConnectionString);
        });
    })
    .Build();

host.Run();
