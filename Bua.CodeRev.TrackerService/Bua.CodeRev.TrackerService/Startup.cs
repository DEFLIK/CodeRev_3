using Bua.CodeRev.TrackerService.DataAccess;
using Bua.CodeRev.TrackerService.DomainCore;
using Bua.CodeRev.TrackerService.DomainCore.Parser;
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
        services.Configure<TimelineTrackerDataBaseSettings>(
            configuration.GetSection(nameof(TimelineTrackerDataBaseSettings)));
        services.AddSingleton<ITimelineTrackerDataBaseSettings>(sp =>
            sp.GetRequiredService<IOptions<TimelineTrackerDataBaseSettings>>().Value);
        services.AddTransient<ITrackerManager, TrackerManager>();
        services.AddTransient<IRepository, Repository>();
        services.AddTransient<ISerializer, Serializer>();
        services.AddTransient<IParser, Parser>();
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