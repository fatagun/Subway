using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cnadf.Cache.InMemory;
using Cnd.Cache.Redis;
using Cnd.Core.ServiceLifetime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scrutor;

namespace Cnd.Sandbox.Api.YetAnotherPlayground
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

            services.AddInMemoryCache(Configuration);
            services.AddRedisCache(Configuration);

            services.Scan(scan =>
                scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo<ISingletonService>())
                    .UsingRegistrationStrategy(registrationStrategy : RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                    .UsingRegistrationStrategy(registrationStrategy: RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                    .AddClasses(classes => classes.AssignableTo<IScopedService>())
                    .UsingRegistrationStrategy(registrationStrategy: RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

            services.AddControllers();
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
