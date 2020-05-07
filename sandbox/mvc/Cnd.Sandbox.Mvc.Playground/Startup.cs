using System.Reflection;
using Cnadf.Cache.InMemory;
using Cnd.Cache.Redis;
using Cnd.Core.ServiceLifetime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrutor;
using Cnd.Core.Common;
using Cnd.Sandbox.Mvc.Playground.Models;
using Cnd.Sandbox.Mvc.Playground.Events;

namespace Cnadf.Sandbox.Mvc.Playground
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInMemoryCache(Configuration);
            services.AddRedisCache(Configuration);
            services.AddHttpContextAccessor();

            services.AddSingleton<ISubscriberService, SubscriberService>();
            services.AddSingleton<IEventPublisher, EventPublisher>();
            services.AddSingleton<IConsumer<Event<Person>>, ProfileUpdatedConsumer>();
            services.AddSingleton<IConsumer<Event<Person>>, RandomConsumer>();

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

            services.AddControllersWithViews();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
