using System.Text.Json;
using System.Text.Json.Nodes;
using AutoMapper;
using Bua.CodeRev.TrackerService.Contracts;
using Bua.CodeRev.TrackerService.Contracts.Record;
using Bua.CodeRev.TrackerService.DomainCore;
using Bua.CodeRev.TrackerService.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Bua.CodeRev.TrackerService.Controller;

[ApiController]
[Route("api/v1/tracker")]
public class TrackerController: ControllerBase
{
    private readonly ITrackerManager manager;
    private readonly IParser parser;
    private readonly ISerializer serializer;
    private readonly IMapper mapper;
    
    public TrackerController(ITrackerManager manager, IMapper mapper, IParser parser, ISerializer serializer)
    {
        this.manager = manager;
        this.mapper = mapper;
        this.parser = parser;
        this.serializer = serializer;
    }

    [HttpGet("get")]
    public string Get([FromQuery] Guid taskSolutionId)
    {
        var result = manager.Get(taskSolutionId);
        var response = serializer.Serialize(result);
        return response.ToString();
    }

    [HttpPut("save")]
    public string Save([FromBody] JsonElement entity)
    {
        var request = parser.ParseRequestDto(entity);
        manager.Save(request);
        return "ok";
    }
}