namespace Bua.CodeRev.TrackerService.DataAccess;

public class TimelineTrackerDataBaseSettings : ITimelineTrackerDataBaseSettings
{
    public string TimelineCollectionName { get; set; }
    public string ConnectionString { get; set; }
    public string DataBaseName { get; set; }
}