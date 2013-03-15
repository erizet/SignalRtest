using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace HubClient
{
    class Program
    {
        Microsoft.AspNet.SignalR.Client.Connection conn = new Microsoft.AspNet.SignalR.Client.Connection("http://ipv4.fiddler:8082/echo");
        public static void Main(string[] args)
        {
            Console.WriteLine("Press key to start...");
            Console.ReadKey();
            // Connect to the service
            //var hubConnection = new HubConnection("http://localhost:8080/signalr");
            var hubConnection = new HubConnection("http://ipv4.fiddler:8082/signalr"); 


            // Create a proxy to the chat service
            var chat = hubConnection.CreateHubProxy("chat");

            // Print the message when it comes in
            chat.On("addMessage", message => Console.WriteLine(message));

            // Start the connection
            hubConnection.Start(new LongPollingTransport()).Wait();

            string line = null;
            while ((line = Console.ReadLine()) != null)
            {
                // Send a message to the server
                chat.Invoke("Send3", line).ContinueWith(t =>
                {
                    Console.WriteLine("Invoke finished");
                });
                //chat.Invoke<string>("Send2", line).ContinueWith(t =>
                //{
                //    Console.WriteLine("Return value: " + t.Result);
                //});
            }
        }
    }
}
