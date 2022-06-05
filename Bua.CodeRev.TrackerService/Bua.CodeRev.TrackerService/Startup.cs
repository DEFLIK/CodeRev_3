using Bua.CodeRev.TrackerService.DataAccess.Infrastructure;
using Bua.CodeRev.TrackerService.DataAccess.Repositories;
using Bua.CodeRev.TrackerService.DomainCore.Deserialize;
using Bua.CodeRev.TrackerService.DomainCore.Serialize;
using Bua.CodeRev.TrackerService.Infrastructure.Mapping;
using Bua.CodeRev.TrackerService.Services;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;

namespace Bua.CodeRev.TrackerService;

public class Startup
{
    private readonly IConfiguration configuration;

    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<TaskRecordsTrackerDataBaseSettings>(
            configuration.GetSection(nameof(TaskRecordsTrackerDataBaseSettings)));
        services.AddSingleton<ITaskRecordsTrackerDataBaseSettings>(sp =>
            sp.GetRequiredService<IOptions<TaskRecordsTrackerDataBaseSettings>>().Value);
        services.AddTransient<ITrackerManager, TrackerManager>();
        services.AddTransient<IRepository, Repository>();
        services.AddTransient<ISerializer, Serializer>();
        services.AddTransient<IDeserializer, Deserializer>();
        services.AddAutoMapper(typeof(RecordProfile));
        services.AddControllers();
        services.AddApiVersioning(config => { config.ApiVersionReader = new HeaderApiVersionReader("api-version"); });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}