using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using SignalR.Hosting.Self;
using SignalR;
using SignalR.Hosting;
using Common;

namespace SignalRtest
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new ConsoleTraceListener());
            Debug.AutoFlush = true;

            string url = "http://borutv584:8081/";
            var server = new Server(url);

            server.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(10);
            // Map connections
            var mc = server.MapConnection<MyConnection>("/echo");
            //server.MapConnection<Raw>("/raw");
            //server.MapHubs();

            server.Start();
            GC.KeepAlive(server);

            Log.WriteLine("Server running on " +  url);

            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D:
                        server.Stop();
                        break;
                    case ConsoleKey.S:
                        var context = GlobalHost.ConnectionManager.GetConnectionContext<MyConnection>();
                        context.Connection.Broadcast(string.Format("This message is from the server message, sendtime: {0}", DateTime.Now));
                        break;
                    case ConsoleKey.U:
                        server.Start();
                        break;
                    case ConsoleKey.Q:
                        return;
                    default:
                        Log.WriteLine("Unknown command");
                        break;
                }
            }
        }

        public class MyConnection : PersistentConnection
        {
            protected override Task OnConnectedAsync(IRequest request, string connectionId)
            {
                Log.WriteLine(connectionId + " connected");
                return base.OnConnectedAsync(request, connectionId);
            }

            protected override Task OnReceivedAsync(IRequest request, string connectionId, string data)
            {
                Log.WriteLine("From " + connectionId + ": " + data);
                return Connection.Broadcast(data);
            }

            protected override Task OnDisconnectAsync(string connectionId)
            {
                Log.WriteLine(connectionId + " left");
                return base.OnDisconnectAsync(connectionId);
            }
        }
    }
}
