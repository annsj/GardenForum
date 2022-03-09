using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnackisApp.Data;
using SnackisApp.Gateways;
using SnackisApp.HelpMethods;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text.Json;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SnackisApp
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });           

            //F�r Dependency Injection av context f�r databas "Snackis"
            services.AddDbContext<SnackisContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("SnackisContext"))); 

            //F�r DI av Gateways
            services.AddScoped<IForumGateway, ForumGateway>();
            services.AddScoped<ISubjectGateway, SubjectGateway>();
            services.AddScoped<IPostGateway, PostGateway>();
            services.AddScoped<IOffensiveWordsGateway, OffensiveWordsGateway>();

            //F�r DI av klassen Content som inneh�ller metoder f�r att skapa testdata
            services.AddScoped<Content>();

            //F�r DI av HTTP-klient
            services.AddHttpClient<ForumGateway>();
            services.AddHttpClient<SubjectGateway>();
            services.AddHttpClient<PostGateway>();


            services.AddAuthorization(o =>
            {
                o.AddPolicy("MustBeAdmin", policy => policy.RequireRole("Admin"));
                o.AddPolicy("MustBeMember", policy => policy.RequireRole("Medlem"));
            });

            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Admin", "MustBeAdmin");
                options.Conventions.AuthorizeFolder("/PM", "MustBeMember");
                options.Conventions.AuthorizeFolder("/MI", "MustBeMember");
                options.Conventions.AuthorizeFolder("/GM", "MustBeMember");
                options.Conventions.AuthorizePage("/CreatePost", "MustBeMember");
                options.Conventions.AuthorizePage("/Report", "MustBeMember");
                options.Conventions.AuthorizePage("/PM/ViewPM", "MustBeMember");
            });




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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
