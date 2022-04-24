using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatServer
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> Connections = new();

        public Task SendMessage(string user, string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public Task Enter(string user)
        {
            Connections[user] = Context.ConnectionId;
            return Clients.All.SendAsync("ReceiveMessage", user, $"{user} has entered chat");
        }
        public async Task JoinGroup(string groupName, string user)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessageFromGroup", groupName, user, $"{user} has joined the group");
        }
        public async Task LeaveGroup(string groupName, string user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessageFromGroup", groupName, user, $"{user} has left the group");
        }
        public Task SendMessageToGroup(string groupName, string user, string message)
        {
            return Clients.OthersInGroup(groupName).SendAsync("ReceiveMessageFromGroup", groupName, user, message);
        }
        public Task SendMessageToUser(string user, string message, string receiver)
        {
            return Clients.Client(Connections[receiver]).SendAsync("ReceiveDirectMessage", user, message);
        }
    }
}
