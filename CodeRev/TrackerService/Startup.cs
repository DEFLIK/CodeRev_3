using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using TrackerService.DataAccess.Infrastructure;
using TrackerService.DataAccess.Repositories;
using TrackerService.DomainCore.Deserialize;
using TrackerService.DomainCore.Serialize;
using TrackerService.EventHandling;
using TrackerService.Services;

namespace TrackerService;

public class Startup
{
    private readonly IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<ValidationMiddleware>();
        services.Configure<TaskRecordsTrackerDataBaseSettings>(
            configuration.GetSection(nameof(TaskRecordsTrackerDataBaseSettings)));
        services.AddSingleton<ITaskRecordsTrackerDataBaseSettings>(sp =>
                                                                       sp.GetRequiredService<IOptions<TaskRecordsTrackerDataBaseSettings>>().Value);
        services.AddTransient<ITrackerManager, TrackerManager>();
        services.AddTransient<IRepository, Repository>();
        services.AddTransient<ISerializer, Serializer>();
        services.AddTransient<IDeserializer, Deserializer>();

        services.AddControllers();
        // services.AddApiVersioning(config => { config.ApiVersionReader = new HeaderApiVersionReader("api-version"); }); // todo repair versioning
        services.AddCors();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseCors(builder => builder.AllowAnyOrigin()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
        app.UseMiddleware<ValidationMiddleware>();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}