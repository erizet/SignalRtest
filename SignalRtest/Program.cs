using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Common;
using Microsoft.Owin.Hosting;
using Microsoft.AspNet.SignalR;
using Owin;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRtest
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://*:8081/";

            // Start hub
            IDisposable wa = WebApplication.Start<Startup>(url);
            try
            {


                Debug.Listeners.Add(new ConsoleTraceListener());
                Debug.AutoFlush = true;

                //string url = "http://localhost:8080";

                {
                    Console.WriteLine("Server running on {0}", url);

                    while (true)
                    {
                        switch (Console.ReadKey().Key)
                        {
                            case ConsoleKey.A:
                                //server.Stop();
                                break;
                            case ConsoleKey.R:
                                //server.Stop();
                                break;
                            case ConsoleKey.D:
                                wa.Dispose();
                                wa=null;
                                break;
                            case ConsoleKey.S:
                                var context = GlobalHost.ConnectionManager.GetConnectionContext<MyConnection>();
                                context.Connection.Broadcast(string.Format("This message is from the server message, sendtime: {0}", DateTime.Now));
                                break;
                            case ConsoleKey.U:
                                if (wa == null)
                                    wa = WebApplication.Start<Startup>(url);
                                break;
                            case ConsoleKey.Q:
                                return;
                            default:
                                Log.WriteLine("Unknown command");
                                break;
                        }
                    }

                }

            }
            finally
            {
                wa.Dispose();

                //server.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(10);

            }
        }

        class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.MapConnection<MyConnection>("/echo");
                app.MapHubs();
            }
        }

        public class MyConnection : PersistentConnection
        {
            protected override Task OnConnectedAsync(IRequest request, string connectionId)
            {
                Log.WriteLine(connectionId + " connected");
                return Groups.Add(connectionId, "foo");
            }

            public override Task ProcessRequestAsync(HostContext context)
            {
                return base.ProcessRequestAsync(context);
            }

            protected override Task OnReceivedAsync(IRequest request, string connectionId, string data)
            {
                Log.WriteLine("From " + connectionId + ": " + data);
                return Connection.Broadcast(data);
            }

            protected override Task OnReconnectedAsync(IRequest request, string connectionId)
            {
                Log.WriteLine(connectionId + " reconnected");
                return base.OnReconnectedAsync(request, connectionId);
            }

            protected override Task OnDisconnectAsync(IRequest request, string connectionId)
            {
                Log.WriteLine(connectionId + " left");
                return Groups.Remove(connectionId, "foo");
            }

            protected override IEnumerable<string> OnRejoiningGroups(IRequest request, IEnumerable<string> groups, string connectionId)
            {
                return groups;
            }
        }


        public class Chat : Hub
        {
            public void Send(string message)
            {
                // Call the addMessage method on all clients            
                Clients.All.addMessage(message);
            }

            public override Task OnConnected()
            {
                return Clients.All.joined(Context.ConnectionId, DateTime.Now.ToString());
            }

            public override Task OnDisconnected()
            {
                return Clients.All.leave(Context.ConnectionId, DateTime.Now.ToString());
            }

            public override Task OnReconnected()
            {
                return Clients.All.rejoined(Context.ConnectionId, DateTime.Now.ToString());
            }

        }



    }
}
