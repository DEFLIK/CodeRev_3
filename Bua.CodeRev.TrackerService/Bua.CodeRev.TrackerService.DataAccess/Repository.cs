using Bua.CodeRev.TrackerService.Contracts.Record;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bua.CodeRev.TrackerService.DataAccess;

public class Repository: IRepository
{
    private readonly IMongoCollection<RecordsRequestDto> timelines;

    public Repository(ITimelineTrackerDataBaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DataBaseName);

        timelines = database.GetCollection<RecordsRequestDto>(settings.TimelineCollectionName);
    }

    public RecordsRequestDto Get(Guid taskSolutionId)
    {
        return timelines.Find(record=>record.TaskSolutionId == taskSolutionId).FirstOrDefault();
    }

    public void Create(RecordsRequestDto request)
    {
        timelines.InsertOne(request);
    }
}