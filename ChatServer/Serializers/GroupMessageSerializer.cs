using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace ChatServer.Serializers
{
    public class GroupMessage
    {
        public string Group { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }
        //public GroupMessage(string Group = "Unknown", string Name = "Unknown", string Message = "empty")
        public GroupMessage()
        {
            Group = "Unknown";
            Name = "Unknown";
            Message = "empty";
        }

        public GroupMessage(string group, string name, string message)
        {
            Group = group;
            Name = name;
            Message = message;
        }
    }
    public static class GroupMessageSerializer
    {
        public static void SerializeMessage(GroupMessage groupMessage)
        {
            /*string fileName = groupMessage.group + ".json";
            var messages = new List<GroupMessage>();
            if (File.Exists(fileName))
            {
                messages = JsonSerializer.Deserialize<List<GroupMessage>>(File.ReadAllText(fileName));
            }*/
            var messages = DeSerializeMessage(groupMessage.Group);

            messages.Add(groupMessage);
            string jsonString = JsonSerializer.Serialize(messages);
            //File.WriteAllText(fileName, jsonString);
            File.WriteAllText("RoomDataBases/" + groupMessage.Group + ".json", jsonString);
        }

        public static List<GroupMessage> DeSerializeMessage(string groupName)
        {
            var fileName = "RoomDataBases/" + groupName + ".json";
            var deserializedMessages = new List<GroupMessage>();
            if (File.Exists(fileName))
            {
                string toSerialize = File.ReadAllText(fileName);
                if (toSerialize.Length > 0)
                {
                    deserializedMessages = JsonSerializer.Deserialize<List<GroupMessage>>(toSerialize);
                }
            }

            /*string[] messageStrings = File.ReadAllLines(fileName);
            var deserialyzedMessages = new GroupMessage[messageStrings.Length];
            for (var i = 0; i < messageStrings.Length; ++i)
            {
                deserialyzedMessages[i] = JsonSerializer.Deserialize<GroupMessage>(messageStrings[i]);
            }*/
            
            return deserializedMessages;
        }
    }
}
