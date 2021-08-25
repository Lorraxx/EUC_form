using EUC_form.DAL;
using EUC_form.DAL.Templates;
using EUC_form.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EUC_form
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Localization";
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("en-US");
                options.AddSupportedUICultures("en-US", "cs-CZ");
                options.FallBackToParentUICultures = true;

                var requestProvider = options.RequestCultureProviders.OfType<AcceptLanguageHeaderRequestCultureProvider>().First();
                options.RequestCultureProviders.Remove(requestProvider);
            });

            #region Repository injection
            // Entity Framework Repository ( uses the ApplicationDBContext )
            services.AddScoped<IContactDetailsRepository, ContactDetailsRepository>();

            // JSON Repository ( uses a JSON file )
            //services.AddScoped<IContactDetailsRepository, JSONContactDetailsRepository>();
            #endregion

            services.AddControllersWithViews();

            services
             .AddRazorPages()
             .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
               opts => opts.ResourcesPath = "Localization");
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
            app.UseRequestLocalization();

            app.UseRouting();

            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                var lang = context.Request.Path.Value.Split('/').Skip(1).First();
                //lang = lang == "cs" ? "cs-CZ" : "en-US";
                CultureInfo.CurrentCulture = new CultureInfo(lang);
                CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;

                await next.Invoke();
            });
        
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{language}/{controller}/{action}/{id}",
                    new { language = "en-US", controller = "ContactDetails", action = "Index", id = "" });
            });
        }
    }
}
