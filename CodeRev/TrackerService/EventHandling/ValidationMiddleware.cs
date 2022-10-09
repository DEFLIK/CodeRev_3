using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace TrackerService.EventHandling;

public class ValidationMiddleware : IMiddleware
{
    private const string ContentType = "application/json";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (ValidationException exception)
        {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = ContentType;

            var responseBody = ToJson(exception);
            await context.Response.WriteAsJsonAsync(responseBody);
        }
    }

    private static JsonObject ToJson(in ValidationException exception)
    {
        return new()
        {
            {"errors", "Validation exception"},
            {"message", exception.Message},
            {"status", StatusCodes.Status400BadRequest}
        };
    }
}