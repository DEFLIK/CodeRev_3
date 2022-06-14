using Bua.CodeRev.TrackerService.Contracts;
using Bua.CodeRev.TrackerService.Contracts.Record;
using Bua.CodeRev.TrackerService.DomainCore.Deserialize;
using Bua.CodeRev.TrackerService.DomainCore.Serialize;
using Bua.CodeRev.TrackerService.Services;
using Bua.CodeRev.TrackerService.Validation;
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
}