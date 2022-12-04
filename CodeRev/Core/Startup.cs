using CompilerService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TrackerService.DataAccess.Infrastructure;
using TrackerService.DataAccess.Repositories;
using TrackerService.DomainCore.Deserialize;
using TrackerService.DomainCore.Serialize;
using TrackerService.EventHandling;
using TrackerService.Hubs;
using TrackerService.Services;
using UserService;
using UserService.DAL;
using UserService.DAL.Models.Interfaces;
using UserService.DAL.Repositories;
using UserService.Helpers;
using UserService.Helpers.Creators;

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
            // todo сделать нормальную настройку конфигурации
            services.AddDbContext<DataContext>(options => options.UseNpgsql(Configuration.GetConnectionString("default"),
                assembly => assembly.MigrationsAssembly("UserService.DAL")));
            services.AddScoped<IDbRepository, DbRepository>();
            services.AddScoped<IInterviewCreator, InterviewCreator>();
            services.AddScoped<ITaskCreator, TaskCreator>();
            services.AddScoped<IInvitationValidator, InvitationValidator>();
            services.AddScoped<IInvitationCreator, InvitationCreator>();
            services.AddScoped<IUserCreator, UserCreatorWithoutUniqueValidations>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<ITokenHelper, TokenHelper>();
            services.AddScoped<IInterviewHelper, InterviewHelper>();
            services.AddScoped<ITaskHelper, TaskHelper>();
            services.AddScoped<ICardHelper, CardHelper>();
            services.AddScoped<IReviewerDraftCreator, ReviewerDraftCreator>();
            services.AddScoped<IDraftHelper, DraftHelper>();
            
            services.AddScoped<IStatusChecker, StatusChecker>();
            services.AddScoped<IMeetsHelper, MeetsHelper>();
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
            // services.AddApiVersioning(config => { config.ApiVersionReader = new HeaderApiVersionReader("api-version"); });
            
            // CompilerService
            services.AddTransient<ICompilerService, CompilerService.Services.CompilerService>();

            services.AddCors();
            services.AddSignalR();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignalRtcHub>("/signalrtc");
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
