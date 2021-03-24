using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorldTrackerDomain.Aggregates;
using WorldTrackerDomain.Commands;
using WorldTrackerDomain.Configuration;
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

            const int MaxRequestSize = 209715200;

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = MaxRequestSize;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = MaxRequestSize; // if don't set default value is: 30 MB
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = MaxRequestSize;
                options.MultipartBodyLengthLimit = MaxRequestSize; // if don't set default value is: 128 MB
                options.MultipartHeadersLengthLimit = MaxRequestSize;
            });

            services.AddControllersWithViews()
                .AddSessionStateTempDataProvider();

            services.AddSession();

            services.AddOptions();

            services.AddMediatR(typeof(PersonCreateCommand));

            services.AddTransient<IBlobUploader, BlobUploader>();
            services.AddTransient<IDomainEventRepository, DomainEventRepository>();
            services.AddTransient<IDomainViewReaderRepository, DomainViewReaderRepository>();
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

            app.UseAuthorization();

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
