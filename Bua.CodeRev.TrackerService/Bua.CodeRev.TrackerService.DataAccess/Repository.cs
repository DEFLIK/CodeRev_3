using Bua.CodeRev.TrackerService.Contracts.Record;
using MongoDB.Driver;

namespace Bua.CodeRev.TrackerService.DataAccess;

public class Repository : IRepository
{
    private readonly IMongoCollection<RecordsRequestDto> timelines;

    public Repository(ITimelineTrackerDataBaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DataBaseName);

        timelines = database.GetCollection<RecordsRequestDto>(settings.TimelineCollectionName);
    }

    public RecordsRequestDto? Get(Guid taskSolutionId)
    {
        return timelines.Find(record => record.TaskSolutionId == taskSolutionId)
            .FirstOrDefault();
    }

    public void Save(RecordsRequestDto? request)
    {
        var record = timelines.Find(x => x.TaskSolutionId == request.TaskSolutionId)
            .FirstOrDefault();
        if (record == null)
            timelines.InsertOne(request);
        else
            Update(new RecordsRequestDto
            {
                TaskSolutionId = record.TaskSolutionId,
                Id = record.Id,
                Code = request.Code,
                RecordChunks = record.RecordChunks.ToList().Concat(request.RecordChunks).ToArray()
            });
    }

    public void Update(RecordsRequestDto request)
    {
        var filter = Builders<RecordsRequestDto>.Filter.Eq(x => x.TaskSolutionId, request.TaskSolutionId);
        var update = Builders<RecordsRequestDto>.Update.Set(x => x.RecordChunks, request.RecordChunks)
            .Set(x => x.Code, request.Code);
        timelines.UpdateOne(filter, update);
    }
}