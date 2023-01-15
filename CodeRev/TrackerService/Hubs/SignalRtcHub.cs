using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using TrackerService.Contracts.Webcam;

namespace TrackerService.Hubs;

public class SignalRtcHub: Hub
{
    public async Task NewUser(string name, string roomName)
    {
        var userInfo = new UserInfo {userName = name, connectionId = Context.ConnectionId};
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.OthersInGroup(roomName).SendAsync("NewUserArrived", JsonSerializer.Serialize(userInfo));
    }

    public async Task HelloUser(string userName, string roomName, string user)
    {
        var userInfo = new UserInfo { userName = userName, groupName = roomName, connectionId = Context.ConnectionId };
        await Clients.Client(user).SendAsync("UserSaidHello", JsonSerializer.Serialize(userInfo));
    }

    public async Task SendSignal(string signal, string user)
    {
        await Clients.Client(user).SendAsync("SendSignal", Context.ConnectionId, signal);
    }

    public async Task SendData(string userConnectionId, string data)
    {
        await Clients.Client(userConnectionId).SendAsync("SendData", userConnectionId, data);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Groups.RemoveFromGroupAsync(Context.ConnectionId,
        await Clients.All.SendAsync("UserDisconnect", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}