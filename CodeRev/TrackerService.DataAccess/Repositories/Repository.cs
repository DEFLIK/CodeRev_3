using MongoDB.Driver;
using TrackerService.Contracts.Record;
using TrackerService.DataAccess.Infrastructure;

namespace TrackerService.DataAccess.Repositories;

public class Repository : IRepository
{
    private readonly IMongoCollection<TaskRecordDto> taskRecords;

    public Repository(ITaskRecordsTrackerDataBaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DataBaseName);

        taskRecords = database.GetCollection<TaskRecordDto>(settings.TaskRecordsCollectionName);
    }

    public async Task<TaskRecordDto?> Get(Guid taskSolutionId)
    {
        var records = await taskRecords.FindAsync(record => record.TaskSolutionId == taskSolutionId)
                                       .ConfigureAwait(false);
        return records.FirstOrDefault();
    }

    public async Task Save(TaskRecordDto request)
    {
        var record = await Get(request.TaskSolutionId);
        if (record == null)
            await taskRecords.InsertOneAsync(request);
        else
            await Update(new TaskRecordDto
            {
                TaskSolutionId = record.TaskSolutionId,
                Id = record.Id,
                Code = request.Code,
                RecordChunks = record.RecordChunks.ToList().Concat(request.RecordChunks).ToArray()
            });
    }

    private async Task Update(TaskRecordDto request)
    {
        var filter = Builders<TaskRecordDto>.Filter.Eq(x => x.TaskSolutionId, request.TaskSolutionId);
        var update = Builders<TaskRecordDto>.Update.Set(x => x.RecordChunks, request.RecordChunks)
                                            .Set(x => x.Code, request.Code);
        await taskRecords.UpdateOneAsync(filter, update);
    }
}