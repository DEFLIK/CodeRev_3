using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TrackerService.Contracts;
using TrackerService.Contracts.Record;
using TrackerService.Infrastructure.Deserialize;
using TrackerService.Infrastructure.Serialize;
using TrackerService.Services;
using TrackerService.Validation;

namespace TrackerService.Controller.v1;

[ApiController]
[EnableCors]
// [ApiVersion("1.0")]
// [Route("api/v{api-version:apiVersion}/tracker")] // todo repair versioning
[Route("api/v1.0/tracker")]
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
    public async Task<RecordChunkResponseDto[]> Get([FromQuery] Guid taskSolutionId, [FromQuery] decimal? saveTime)
    {
        var result = await manager.Get(taskSolutionId, saveTime);
        Validator.NotNull(result, nameof(taskSolutionId), $"Not found {nameof(taskSolutionId)}: {taskSolutionId}");
        var response = serializer.Serialize(result);
        return response;
    }

    [HttpGet("get-last-code")]
    public async Task<LastCodeDto> GetLastCode([FromQuery] Guid taskSolutionId)
    {
        var result = await manager.GetLastCode(taskSolutionId);
        Validator.NotNull(result, nameof(taskSolutionId), $"Not found {nameof(taskSolutionId)}: {taskSolutionId}");
        return result;
    }

    [HttpPut("save")]
    public async Task Save([FromBody] TaskRecordRequestDto requestDto)
    {
        var request = deserializer.ParseRequestDto(requestDto);
        TaskRecordRequestValidator.Validate(request);
        await manager.Save(request);
    }

    [HttpPost("save-video")]
    public async Task SaveVideo([FromBody] JsonValue obj)
    {
        Console.WriteLine(obj);
    }
}