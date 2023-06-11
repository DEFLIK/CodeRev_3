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
using TaskTestsProvider;
using TrackerService.DataAccess.Infrastructure;
using TrackerService.DataAccess.Repositories;
using TrackerService.EventHandling;
using TrackerService.Hubs;
using TrackerService.Infrastructure.Deserialize;
using TrackerService.Infrastructure.Serialize;
using TrackerService.Services;
using UserService;
using UserService.DAL;
using UserService.DAL.Models.Interfaces;
using UserService.DAL.Repositories;
using UserService.Helpers;
using UserService.Helpers.Auth;
using UserService.Helpers.Auth.Invitations;
using UserService.Helpers.Interviews;
using UserService.Helpers.Notifications;
using UserService.Helpers.Tasks;

namespace Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureUserService(services);
            ConfigureTrackerService(services);
            ConfigureCompilerService(services);

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext context)
        {
            var isDevEnv = Environment.IsDevelopment();
            // context.Database.Migrate();
            
            if (isDevEnv)
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core v1"));
            }

            //ToDo решить вопроскики
            // app.UseHttpsRedirection();

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
                endpoints.MapHub<NotificationHub>("/notificationHub");
                endpoints.MapControllers();
            });

            app.UseStaticFiles();
            if (!isDevEnv)
                app.UseSpaStaticFiles();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "../WebClient";

                if (isDevEnv)
                    spa.UseAngularCliServer("start");
            });

        }

        private void ConfigureCompilerService(IServiceCollection services)
        {
            services.AddTransient<CSharpCompilerService>();
            services.AddTransient<JsCompilerService>();
            services.AddTransient<AssemblyTestingService>();
        }

        private void ConfigureTrackerService(IServiceCollection services)
        {
            services.AddTransient<ValidationMiddleware>();

            var dataBaseSettingsConfig = nameof(TaskRecordsTrackerDataBaseSettings) + Environment.EnvironmentName;
            services.Configure<TaskRecordsTrackerDataBaseSettings>(Configuration.GetSection(dataBaseSettingsConfig));
            services.AddSingleton<ITaskRecordsTrackerDataBaseSettings>(sp => sp.GetRequiredService<IOptions<TaskRecordsTrackerDataBaseSettings>>().Value);
            
            services.AddTransient<ITrackerManager, TrackerManager>();
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<ISerializer, Serializer>();
            services.AddTransient<IDeserializer, Deserializer>();
            // services.AddApiVersioning(config => { config.ApiVersionReader = new HeaderApiVersionReader("api-version"); });
        }

        private void ConfigureUserService(IServiceCollection services)
        {
            var postgresConnectionString = Configuration.GetConnectionString($"postgres{Environment.EnvironmentName}");
            services.AddDbContext<DataContext>(options => options.UseNpgsql(postgresConnectionString,
                assembly => assembly.MigrationsAssembly("UserService.DAL")));

            services.AddScoped<ITaskTestsProviderClient, TaskTestsProviderClient>();
            
            services.AddScoped<IDbRepository, DbRepository>();
            services.AddScoped<IInterviewCreator, InterviewCreator>();
            services.AddScoped<ITaskCreator, TaskCreator>();
            services.AddScoped<IInvitationValidator, InvitationValidator>();
            services.AddScoped<IInvitationCreator, InvitationCreator>();
            services.AddScoped<IUserCreator, UserCreator>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IInterviewHelper, InterviewHelper>();
            services.AddScoped<ITaskHelper, TaskHelper>();
            services.AddScoped<ICardHelper, CardHelper>();
            services.AddScoped<IReviewerDraftCreator, ReviewerDraftCreator>();
            services.AddScoped<IDraftHelper, DraftHelper>();
            services.AddScoped<IStatusChecker, StatusChecker>();
            services.AddScoped<IMeetsHelper, MeetsHelper>();
            services.AddScoped<INotificationsHelper, NotificationsHelper>();
            services.AddScoped<INotificationsCreator, NotificationsCreator>();
            services.AddScoped<TelegramBotHelper, TelegramBotHelper>();
            services.AddScoped<NotificationHub, NotificationHub>();
            services.AddScoped<NotificationMassageBuilder, NotificationMassageBuilder>();
            services.AddScoped<ITaskHandler, TaskHandler>();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; //todo make true for using ssl
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
        }
    }
}
