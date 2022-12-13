using TrackerService.EventHandling;

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