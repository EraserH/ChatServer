using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace ChatServer.Serializers
{
    public class DirectMessage
    {
        public string Receiver { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }
        //public GroupMessage(string Group = "Unknown", string Name = "Unknown", string Message = "empty")
        public DirectMessage()
        {
            Receiver = "Unknown";
            Name = "Unknown";
            Message = "empty";
        }

        public DirectMessage(string receiver, string name, string message)
        {
            Receiver = receiver;
            Name = name;
            Message = message;
        }
    }

    public class DirectMessageSerializer
    {
        public static void SerializeMessage(DirectMessage directMessage)
        {
            var messages = DeSerializeMessage(directMessage.Receiver);

            messages.Add(directMessage);
            string jsonString = JsonSerializer.Serialize(messages);
            File.WriteAllText("DirectDataBases/" + directMessage.Receiver + ".json", jsonString);
        }

        public static List<DirectMessage> DeSerializeMessage(string receiverName)
        {
            var fileName = "DirectDataBases/" + receiverName + ".json";
            var deserializedMessages = new List<DirectMessage>();
            if (File.Exists(fileName))
            {
                string toSerialize = File.ReadAllText(fileName);
                if (toSerialize.Length > 0)
                {
                    deserializedMessages = JsonSerializer.Deserialize<List<DirectMessage>>(toSerialize);
                }
            }

            return deserializedMessages;
        }
    }
}
