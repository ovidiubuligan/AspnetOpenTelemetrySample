using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServiceB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseKestrel(options =>
                    //{
                    //    var devOverride = Environment.GetEnvironmentVariable("ASPNETCORE_DEVELOPER_OVERRIDES");
     
                    //    options.ListenAnyIP(80);

                    //})
                    webBuilder.UseStartup<Startup>();
                });
    }
}
