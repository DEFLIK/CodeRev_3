namespace TrackerService.DataAccess.Infrastructure;

public interface ITaskRecordsTrackerDataBaseSettings
{
    string TaskRecordsCollectionName { get; set; }
    string ConnectionString { get; set; }
    string DataBaseName { get; set; }
}