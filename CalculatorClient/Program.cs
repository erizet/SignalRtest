using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorClient
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
            var calc = hubConnection.CreateHubProxy("calculator");

            // Print the message when it comes in
            //chat.On("addMessage", message => Console.WriteLine(message));

            // Start the connection
            hubConnection.Start(new LongPollingTransport()).Wait();

            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.A:
                        calc.Invoke<int>("Add", new object[] { 1, 2 }).ContinueWith(t => Console.WriteLine("Addition: " + t.Result.ToString()));
                        break;
                    case ConsoleKey.B:
                        calc.Invoke<int>("Sub", new object[] { 1, 2 }).ContinueWith(t => Console.WriteLine("Subtraktion: " + t.Result.ToString()));
                        break;
                    case ConsoleKey.C:
                        calc.Invoke<string>("GetName", new object[0]).ContinueWith(t => Console.WriteLine("Name: " + t.Result));
                        break;
                    case ConsoleKey.M:
                        calc.Invoke<string>("Mix", new object[] { 1, "kalle" }).ContinueWith(t => Console.WriteLine("Mixat: " + t.Result));
                        break;

                    case ConsoleKey.Q:
                        return;
                    default:
                        break;
                }
            }
        }
    }
}
