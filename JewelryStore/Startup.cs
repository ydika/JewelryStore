using JewelryStore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataBaseContext>();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllersWithViews(mvcOtions => mvcOtions.EnableEndpointRouting = false);

            services.AddWebOptimizer(pipeline =>
            {
                pipeline.MinifyCssFiles("/inline/css/*.css");
                //pipeline.MinifyJsFiles("/inline/js/*.js");

                pipeline.AddCssBundle("/css/bundle.css", "/css/*.css");
                pipeline.AddJavaScriptBundle("/js/bundle.js", "/js/*.js");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                app.UseHsts();
            }
            app.UseWebOptimizer();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseMvcWithDefaultRoute();
        }
    }
}