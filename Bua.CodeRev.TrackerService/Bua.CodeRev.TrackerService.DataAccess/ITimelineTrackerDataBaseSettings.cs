namespace Bua.CodeRev.TrackerService.DataAccess;

public interface ITimelineTrackerDataBaseSettings
{
    string TimelineCollectionName { get; set; }
    string ConnectionString { get; set; }
    string DataBaseName { get; set; }
}