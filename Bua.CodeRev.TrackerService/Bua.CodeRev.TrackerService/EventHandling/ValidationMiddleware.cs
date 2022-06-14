using System.ComponentModel.DataAnnotations;

namespace Bua.CodeRev.TrackerService.EventHandling;

public class ValidationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (ValidationException exception)
        {
            const string message = "An unhandled exception has occurred while executing the request.";

            const string contentType = "application/json";
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = contentType;

            var json = ToJson(exception);
            await context.Response.WriteAsync(json);
        }
    }

    private static string ToJson(in Exception exception)
    {
        return $"status code:400\n {exception.Message}";
    }
}