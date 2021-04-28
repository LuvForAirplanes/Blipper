using Blipper.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blipper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var instance = CreateHostBuilder(args).Build();

            var scope = instance.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetService<TelegramRelayService>();
            seeder.InitService();

            instance.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
