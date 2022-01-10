using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SIGNAL_R_CHAT.API.Hubs;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SIGNAL_R_CHAT.Domain;
using Microsoft.AspNetCore.Http;

namespace SIGNAL_R_CHAT.API
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
            services.AddDbContext<Infrastructure.Context>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Signal-R-CHATConString")));

            services.AddIdentity<Person, IdentityRole>()
                .AddEntityFrameworkStores<Infrastructure.Context>();


            //add SameSite flag to identity cookies
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
            });

            services.AddAntiforgery(opts => {
                opts.Cookie.SameSite = SameSiteMode.Unspecified;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("LocalPolicy",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44351")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowCredentials();
           
                    });
            });

            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();
            services.AddSignalR();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SIGNAL_R_CHAT.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SIGNAL_R_CHAT.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("LocalPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
