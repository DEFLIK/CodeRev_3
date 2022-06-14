using System.ComponentModel.DataAnnotations;
using Bua.CodeRev.TrackerService.Contracts;
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
    public async Task<RecordChunkResponseDto[]> Get([FromQuery] Guid taskSolutionId, [FromQuery] decimal? saveTime)
    {
        var result = await manager.Get(taskSolutionId, saveTime);
        if (result == null)
            throw new ValidationException($"Не найден id: {taskSolutionId}");
        var response = serializer.Serialize(result);
        // throw new BadHttpRequestException(
        //     $"Not found {nameof(TaskRecordDto)} with taskSolutionId: {taskSolutionId}");
        return response;
    }

    [HttpGet("get-last-code")]
    public async Task<LastCodeDto?> GetLastCode([FromQuery] Guid taskSolutionId)
    {
        return await manager.GetLastCode(taskSolutionId);
    }

    [HttpPut("save")]
    public async Task Save([FromBody] TaskRecordRequestDto requestDto)
    {
        var request = deserializer.ParseRequestDto(requestDto);
        await manager.Save(request);
    }
}