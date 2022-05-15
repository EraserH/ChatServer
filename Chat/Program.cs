using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;

namespace Chat
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var userName = AnsiConsole.Ask<string>("What's your username?");

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chatroom")
                .Build();

            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                AnsiConsole.MarkupLine($"[bold yellow]{user}[/]: [blue]{message}[/]");
            });

            connection.On<string, string, string>("ReceiveMessageFromGroup", (groupName, user, message) =>
            {
                AnsiConsole.MarkupLine($"([bold red]{groupName}[/]) [bold yellow]{user}[/]: [blue]{message}[/]");
            });

            connection.On<string, string>("ReceiveDirectMessage", (user, message) =>
            {
                AnsiConsole.MarkupLine($"[bold red]{user}[/]: [blue]{message}[/]");
            });



            await connection.StartAsync();

            await connection.InvokeAsync("Enter", userName);

            while (true)
            {
                var message = AnsiConsole.Ask<string>($"{userName}: ");
                if (message == "exit")
                    break;

                if (message.StartsWith("+"))
                    await connection.InvokeAsync("JoinGroup", message.Split('+', ' ')[1], userName);
                else if (message.StartsWith("-"))
                    await connection.InvokeAsync("LeaveGroup", message.Split('-', ' ')[1], userName);
                else if (message.StartsWith("#"))
                {
                    var groupName = message.Split('#', ' ')[1];
                    var messageToSend = message.Replace("#" + groupName, "");
                    await connection.InvokeAsync("SendMessageToGroup", groupName, userName, messageToSend);
                }
                else if (message.StartsWith("@"))
                {
                    var receiver = message.Split('@', ' ')[1];
                    var messageToSend = message.Replace("@" + receiver, "");
                    await connection.InvokeAsync("SendMessageToUser", userName, messageToSend, receiver);
                }
                else
                    await connection.InvokeAsync("SendMessage", userName, message);
            }

            await connection.StopAsync();
        }
    }
}
