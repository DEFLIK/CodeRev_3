using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using TrackerService.Contracts.Webcam;

namespace TrackerService.Hubs;

public class NotificationHub: Hub
{
    public async Task NewUser(string name, string roomName)
    {
        var userInfo = new UserInfo {userName = name, connectionId = Context.ConnectionId};
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.OthersInGroup(roomName).SendAsync("NewUserArrived", JsonSerializer.Serialize(userInfo));
    }
    
    public async Task SendNotification(string notification)
    {
        await Clients.All.SendAsync("SendNotification", notification);
    }
    
    public async Task InvokeSendNotification(string notification)
    {
        await Clients.All.SendAsync("SendNotification", notification);
    }
}