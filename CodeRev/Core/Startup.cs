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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
            context.Database.Migrate();
            
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
                endpoints.MapHub<NotificationHub>("/notificationHub");
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

        private void ConfigureCompilerService(IServiceCollection services)
        {
            services.AddTransient<ICompilerService, CompilerService.Services.CompilerService>();
        }

        private void ConfigureTrackerService(IServiceCollection services)
        {
            services.AddTransient<ValidationMiddleware>();
            services.Configure<TaskRecordsTrackerDataBaseSettings>(Configuration.GetSection(nameof(TaskRecordsTrackerDataBaseSettings)));
            services.AddSingleton<ITaskRecordsTrackerDataBaseSettings>(sp => sp.GetRequiredService<IOptions<TaskRecordsTrackerDataBaseSettings>>().Value);
            services.AddTransient<ITrackerManager, TrackerManager>();
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<ISerializer, Serializer>();
            services.AddTransient<IDeserializer, Deserializer>();
            // services.AddApiVersioning(config => { config.ApiVersionReader = new HeaderApiVersionReader("api-version"); });
        }

        private void ConfigureUserService(IServiceCollection services)
        {
            // todo сделать нормальную настройку конфигурации базы
            services.AddDbContext<DataContext>(options => options.UseNpgsql(Configuration.GetConnectionString("default"),
                assembly => assembly.MigrationsAssembly("UserService.DAL")));
            
            services.AddScoped<IDbRepository, DbRepository>();
            services.AddScoped<IInterviewCreator, InterviewCreator>();
            services.AddScoped<ITaskCreator, TaskCreator>();
            services.AddScoped<IInvitationValidator, InvitationValidator>();
            services.AddScoped<IInvitationCreator, InvitationCreator>();
            services.AddScoped<IUserCreator, UserCreatorWithoutUniqueValidations>();
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
        }
    }
}
