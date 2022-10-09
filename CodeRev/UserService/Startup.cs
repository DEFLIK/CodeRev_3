using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserService.DAL;
using UserService.DAL.Models.Interfaces;
using UserService.DAL.Repositories;

namespace UserService
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // services.Configure<AuthOptions>(_configuration.GetSection("Auth")); // if auth options written in appsettings
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; //todo change to true after dev for using ssl
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.Issuer,
                        
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.Audience,
                        
                        ValidateLifetime = true,
                        
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
                    };
                });

            // services.AddCors(options =>
            // {
            //     options.AddPolicy("AllowAnyOriginAndCredentials", builder =>
            //     {
            //         // builder.WithOrigins("https://localhost:5001", "http://localhost:5000", "http://localhost:62167", 
            //         //         "https://localhost:44393", "https://localhost:44395")
            //         //     .AllowCredentials()
            //         //     .AllowAnyHeader()
            //         //     .AllowAnyMethod()
            //         builder.SetIsOriginAllowed(origin => true).AllowCredentials().AllowAnyHeader().AllowAnyMethod();
            //     });
            // });
            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService v1"));
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();
            
            // app.UseCors("AllowAnyOriginAndCredentials");
            app.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}