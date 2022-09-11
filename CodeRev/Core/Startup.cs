using CompilerService.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using TrackerService.DataAccess.Infrastructure;
using TrackerService.DataAccess.Repositories;
using TrackerService.DomainCore.Deserialize;
using TrackerService.DomainCore.Serialize;
using TrackerService.EventHandling;
using TrackerService.Services;
using UserService.DAL;
using UserService.DAL.Models.Interfaces;
using UserService.DAL.Repositories;

namespace Core
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
            // UserService
            services.AddDbContext<DataContext>(options => options.UseNpgsql(Configuration.GetConnectionString("default"),
                assembly => assembly.MigrationsAssembly("UserService.DAL")));
            services.AddScoped<IDbRepository, DbRepository>();
            
            // TrackerService
            services.AddTransient<ValidationMiddleware>();
            services.Configure<TaskRecordsTrackerDataBaseSettings>(
                Configuration.GetSection(nameof(TaskRecordsTrackerDataBaseSettings)));
            services.AddSingleton<ITaskRecordsTrackerDataBaseSettings>(sp =>
                sp.GetRequiredService<IOptions<TaskRecordsTrackerDataBaseSettings>>().Value);
            services.AddTransient<ITrackerManager, TrackerManager>();
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<ISerializer, Serializer>();
            services.AddTransient<IDeserializer, Deserializer>();
            
            // CompilerService
            services.AddTransient<ICompiler, Compiler>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core", Version = "v1" });
            });

            services.AddSpaStaticFiles(conf =>
            {
                conf.RootPath = "../WebClient/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();
            if (!env.IsDevelopment())
                app.UseSpaStaticFiles();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "../WebClient";

                if (env.IsDevelopment())
                    spa.UseAngularCliServer("start");
            });

        }
    }
}
