using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorldTrackerDomain.Aggregates;
using WorldTrackerDomain.Commands;
using WorldTrackerDomain.Configuration;
using WorldTrackerDomain.Projectors;
using WorldTrackerDomain.Repositories;
using WorldTrackerWeb.Components;

namespace WorldTrackerWeb
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
            services
               .AddOptions<WorldTrackerOptions>()
               .Configure<IConfiguration>((settings, configuration) =>
               {
                   configuration.Bind(nameof(WorldTrackerOptions), settings);
               });

            services.AddControllersWithViews()
                .AddSessionStateTempDataProvider();

            services.AddSession();

            services.AddOptions();

            services.AddMediatR(typeof(PersonCreateCommand));

            services.AddTransient<IBlobUploader, BlobUploader>();
            services.AddTransient<IDomainEventRepository, DomainEventRepository>();
            services.AddTransient<IDomainViewRepository, DomainViewRepository>();
            services.AddTransient<IPersonGetByIDViewProjector, PersonGetByIDViewProjector>();
            services.AddTransient<IPlaceGetByIDViewProjector, PlaceGetByIDViewProjector>();
            services.AddTransient<ISummaryViewProjector, SummaryViewProjector>();
            services.AddTransient<IPersonAggregate, PersonAggregate>();
            services.AddTransient<IPlaceAggregate, PlaceAggregate>();
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

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
