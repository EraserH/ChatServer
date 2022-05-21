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

            var deSerializedDirectMessages = Serializers.DirectMessageSerializer.DeSerializeMessage(user);
            foreach (var directMessage in deSerializedDirectMessages)
            {
                Clients.Client(Connections[user]).SendAsync("ReceiveDirectMessage", directMessage.Name, directMessage.Message);
            }

            return Clients.All.SendAsync("ReceiveServiceMessage", "common", $"{user} has entered chat");
        }
        public async Task JoinGroup(string groupName, string user)
        {
            string message = $"{user} has joined the group";

            var deSerializedGroupMessages = Serializers.GroupMessageSerializer.DeSerializeMessage(groupName);
            foreach (var groupMessage in deSerializedGroupMessages)
            {
                await Clients.Caller.SendAsync("ReceiveMessageFromGroup", groupName, groupMessage.Name, groupMessage.Message);
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            //await Clients.Group(groupName).SendAsync("ReceiveMessageFromGroup", groupName, user, message);
            await Clients.Group(groupName).SendAsync("ReceiveServiceMessage", groupName, message);

            Serializers.GroupMessageSerializer.SerializeMessage(new Serializers.GroupMessage(groupName, user, message));
        }
        public async Task LeaveGroup(string groupName, string user)
        {
            string message = $"{user} has left the group";

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveServiceMessage", groupName, message);

            Serializers.GroupMessageSerializer.SerializeMessage(new Serializers.GroupMessage(groupName, user, message));
        }
        public Task SendMessageToGroup(string groupName, string user, string message)
        {
            Serializers.GroupMessageSerializer.SerializeMessage(new Serializers.GroupMessage(groupName, user, message));
            return Clients.OthersInGroup(groupName).SendAsync("ReceiveMessageFromGroup", groupName, user, message);
        }
        public Task SendMessageToUser(string user, string message, string receiver)
        {
            Serializers.DirectMessageSerializer.SerializeMessage(new Serializers.DirectMessage(receiver, user, message));
            return Clients.Client(Connections[receiver]).SendAsync("ReceiveDirectMessage", user, message);
        }
    }
}
