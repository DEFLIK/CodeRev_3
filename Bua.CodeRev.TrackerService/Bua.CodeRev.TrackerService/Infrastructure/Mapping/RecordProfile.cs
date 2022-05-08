using AutoMapper;

namespace Bua.CodeRev.TrackerService.Infrastructure.Mapping;

public class RecordProfile : Profile
{
    public RecordProfile()
    {
        CreateMap<int, int[]>();
    }
}