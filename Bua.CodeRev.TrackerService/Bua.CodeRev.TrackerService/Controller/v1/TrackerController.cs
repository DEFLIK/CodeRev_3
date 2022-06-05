using System.Text.Json;
using Bua.CodeRev.TrackerService.DomainCore;
using Bua.CodeRev.TrackerService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bua.CodeRev.TrackerService.Controller.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{api-version:apiVersion}/tracker")]
public class TrackerController : ControllerBase
{
    private readonly ITrackerManager manager;
    private readonly IParser parser; //rename
    private readonly ISerializer serializer;

    public TrackerController(ITrackerManager manager, IParser parser, ISerializer serializer)
    {
        this.manager = manager;
        this.parser = parser;
        this.serializer = serializer;
    }

    [HttpGet("get")]
    public string? Get([FromQuery] Guid taskSolutionId, [FromQuery] decimal? saveTime)
    {
        var result = manager.Get(taskSolutionId, saveTime);
        var response = serializer.Serialize(result);
        return response?.ToString();
    }

    [HttpGet("get-last-code")]
    public string? GetLastCode([FromQuery] Guid taskSolutionId)
    {
        return manager.GetLastCode(taskSolutionId);
    }

    [HttpPut("save")]
    public string Save([FromBody] JsonElement entity)
    {
        var request = parser.ParseRequestDto(entity);
        manager.Save(request);
        return "ok";
    }
}