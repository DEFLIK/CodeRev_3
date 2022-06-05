using Bua.CodeRev.TrackerService.Contracts.Record;
using Bua.CodeRev.TrackerService.DataAccess.Infrastructure;
using MongoDB.Driver;

namespace Bua.CodeRev.TrackerService.DataAccess.Repositories;

public class Repository : IRepository
{
    private readonly IMongoCollection<TaskRecordDto> taskRecords;

    public Repository(ITaskRecordsTrackerDataBaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DataBaseName);

        taskRecords = database.GetCollection<TaskRecordDto>(settings.TaskRecordsCollectionName);
    }

    public TaskRecordDto? Get(Guid taskSolutionId)
    {
        return taskRecords.Find(record => record.TaskSolutionId == taskSolutionId)
            .FirstOrDefault();
    }

    public void Save(TaskRecordDto? request)
    {
        var record = taskRecords.Find(x => x.TaskSolutionId == request.TaskSolutionId)
            .FirstOrDefault();
        if (record == null)
            taskRecords.InsertOne(request);
        else
            Update(new TaskRecordDto
            {
                TaskSolutionId = record.TaskSolutionId,
                Id = record.Id,
                Code = request.Code,
                RecordChunks = record.RecordChunks.ToList().Concat(request.RecordChunks).ToArray()
            });
    }

    public void Update(TaskRecordDto request)
    {
        var filter = Builders<TaskRecordDto>.Filter.Eq(x => x.TaskSolutionId, request.TaskSolutionId);
        var update = Builders<TaskRecordDto>.Update.Set(x => x.RecordChunks, request.RecordChunks)
            .Set(x => x.Code, request.Code);
        taskRecords.UpdateOne(filter, update);
    }
}