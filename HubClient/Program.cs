using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace HubClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connect to the service
            var hubConnection = new HubConnection("http://ipv4.fiddler:8081/");

            // Create a proxy to the chat service
            var chat = hubConnection.CreateHubProxy("chat");

            // Print the message when it comes in
            chat.On("addMessage", message => Console.WriteLine(message));

            Console.WriteLine("Press key to start");
            Console.ReadKey();
            // Start the connection
            hubConnection.Start().Wait();

            Console.WriteLine("Started");

            string line = null;
            while ((line = Console.ReadLine()) != null)
            {
                // Send a message to the server
                chat.Invoke("Send", line).Wait();
            }

        }
    }
}
