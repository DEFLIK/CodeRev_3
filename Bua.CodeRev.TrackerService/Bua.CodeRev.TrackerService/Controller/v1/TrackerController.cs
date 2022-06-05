using System.Text.Json;
using Bua.CodeRev.TrackerService.Contracts.Record;
using Bua.CodeRev.TrackerService.DomainCore.Deserialize;
using Bua.CodeRev.TrackerService.DomainCore.Serialize;
using Bua.CodeRev.TrackerService.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Bua.CodeRev.TrackerService.Controller.v1;

[ApiController]
[EnableCors]
[ApiVersion("1.0")]
[Route("api/v{api-version:apiVersion}/tracker")]
public class TrackerController : ControllerBase
{
    private readonly IDeserializer deserializer;
    private readonly ITrackerManager manager;
    private readonly ISerializer serializer;

    public TrackerController(ITrackerManager manager, IDeserializer deserializer, ISerializer serializer)
    {
        this.manager = manager;
        this.deserializer = deserializer;
        this.serializer = serializer;
    }

    [HttpGet("get")]
    public IActionResult Get([FromQuery] Guid taskSolutionId, [FromQuery] decimal? saveTime)
    {
        var result = manager.Get(taskSolutionId, saveTime);
        var response = serializer.Serialize(result);
        if (response == null)
            return NotFound();
            // throw new BadHttpRequestException(
            //     $"Not found {nameof(TaskRecordDto)} with taskSolutionId: {taskSolutionId}");
        return Ok(response.ToString());
    }

    [HttpGet("get-last-code")]
    public string? GetLastCode([FromQuery] Guid taskSolutionId)
    {
        return manager.GetLastCode(taskSolutionId);
    }

    [HttpPut("save")]
    public string Save([FromBody] JsonElement entity)
    {
        var request = deserializer.ParseRequestDto(entity);
        manager.Save(request);
        return "ok";
    }
}