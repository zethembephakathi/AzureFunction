using ABC_Retail.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace ABC_Retail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddSingleton<TableStorage>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var connectionString = config["AzureStorage:ConnectionString"];
                return new TableStorage(connectionString);
            });

            // Register BlobStorage as a singleton
            builder.Services.AddSingleton<BlobStorage>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var connectionString = config["AzureStorage:ConnectionString"];
                return new BlobStorage(connectionString);
            });

            // Register QueueStorage as a singleton
            builder.Services.AddSingleton<QueueStorage>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var connectionString = config["AzureStorage:ConnectionString"];
                return new QueueStorage(connectionString);
            });

            // Register LogFileService as a singleton
            builder.Services.AddSingleton<LogFileService>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var connectionString = config["AzureFileStorage:ConnectionString"];
                var shareName = config["AzureFileStorage:ShareName"];
                return new LogFileService(connectionString, shareName);
            });

          

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

           
            app.UseStaticFiles();

            app.UseRouting();

            

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}