using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using OpenTelemetry;
using Microsoft.AspNetCore.Http;

namespace ServiceA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenTelemetryTracing(
                    (serviceProvider, builder) =>
                    {
                        var serviceName = "A";
                        var resource = OpenTelemetry.Resources.Resources.CreateServiceResource(serviceName);
                        builder
                             .SetResource(resource)
                             .AddSource(serviceName)
                             .SetSampler(new AlwaysOnSampler())
                             .AddHttpClientInstrumentation()
                             .AddAspNetCoreInstrumentation(config => {
                                  config.Enrich = (activity, str, obj) =>
                                  {
                                      if (str == "OnStopActivity" && obj is HttpResponse httpResponse)
                                      {
                                          if (httpResponse.StatusCode >= 500)
                                          {
                                              // in order to show as an error in jaeger it needs error=true tag
                                              activity.SetTag("error", true);
                                          }
                                      }
                                  };
                                  config.RecordException = true;
                              })
                             .AddJaegerExporter((o) =>
                             {
                                 o.AgentHost = "localhost";
                                 o.AgentPort = 6835;
                                 o.ServiceName = serviceName;
                             });

                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
