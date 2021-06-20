using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6
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
       .AddAuthentication(o =>
       {
           o.DefaultScheme = "Application";
           o.DefaultSignInScheme = "External";
       })
       .AddCookie("Application")
       .AddCookie("External")
      .AddSpotify(options =>
      {
          options.ClientId = "9c2b1495bef94287a41476d1a1a44ea2";
          options.ClientSecret = "d0f67e70b01d483787dbf8fa3c8c11b7";
          //options.CallbackPath = "/Account/LoginCallback";

          options.Scope.Add("user-modify-playback-state");
          options.Scope.Add("user-read-playback-state");
          options.Scope.Add("user-top-read");
          options.Scope.Add("user-read-private");
          options.SaveTokens = true;
          options.Events.OnCreatingTicket = ctx =>
          {
              List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();

              tokens.Add(new AuthenticationToken()
              {
                  Name = "TicketCreated",
                  Value = DateTime.UtcNow.ToString()
              });

              ctx.Properties.StoreTokens(tokens);
              return Task.CompletedTask;
          };

      });
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                {
                    cookieContext.CookieOptions.SameSite = SameSiteMode.Unspecified;
                };
                options.OnDeleteCookie = cookieContext =>
                {
                    cookieContext.CookieOptions.SameSite = SameSiteMode.Unspecified;
                };
            });
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
            app.UseCookiePolicy();
            app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
