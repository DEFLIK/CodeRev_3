namespace TrackerService.DataAccess.Infrastructure;

public class TaskRecordsTrackerDataBaseSettings : ITaskRecordsTrackerDataBaseSettings
{
    public string TaskRecordsCollectionName { get; set; }
    public string ConnectionString { get; set; }
    public string DataBaseName { get; set; }
}