using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ef.Seeder;
using efCoreSeederSample.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace efCoreSeederSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var serviceProvider = scope.ServiceProvider;
                new DatabaseSeeder(serviceProvider, serviceProvider.GetService<SeederSampleDbContext>())
                    .IsProductionEnvironment(false)
                    .EnsureSeeded(true);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}