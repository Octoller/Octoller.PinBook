using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Octoller.PinBook.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                Data.DataInitilizer.InitializeAsync(scope.ServiceProvider).GetAwaiter().GetResult();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddJsonFile(
                            path: "appsettings.json",
                            optional: true, reloadOnChange: true);

                        builder.AddJsonFile(
                            path: $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                            optional: true, reloadOnChange: true);

                        builder.AddJsonFile(
                            path: "privatesettings.json",
                            optional: true, reloadOnChange: true);
                    });

                    webBuilder.UseKestrel();
                    webBuilder.UseIIS();

                    webBuilder.UseDefaultServiceProvider((context, options) =>
                    {
                        options.ValidateScopes = false;
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
