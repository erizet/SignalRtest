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

namespace SignalRtest
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new ConsoleTraceListener());
            Debug.AutoFlush = true;

            //string url = "http://localhost:8080";
            string url = "http://*:8082/";
            
            using (var wa = WebApplication.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);

                while (true)
                {
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.D:
                            //server.Stop();
                            break;
                        case ConsoleKey.S:
                            var context = GlobalHost.ConnectionManager.GetConnectionContext<MyConnection>();
                            context.Connection.Broadcast(string.Format("This message is from the server message, sendtime: {0}", DateTime.Now));
                            break;
                        case ConsoleKey.U:
                            //server.Start();
                            break;
                        case ConsoleKey.Q:
                            return;
                        default:
                            Log.WriteLine("Unknown command");
                            break;
                    }
                }

            }



            //server.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(10);


        }

        class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.MapConnection<MyConnection>("/echo");
            }
        }

        public class MyConnection : PersistentConnection
        {
            protected override Task OnConnected(IRequest request, string connectionId)
            {
                Log.WriteLine(connectionId + " connected");
                return base.OnConnected(request, connectionId);
            }

            public override Task ProcessRequest(Microsoft.AspNet.SignalR.Hosting.HostContext context)
            {
                return base.ProcessRequest(context);
            }

            protected override Task OnReceived(IRequest request, string connectionId, string data)
            {
                Log.WriteLine("From " + connectionId + ": " + data);
                return Connection.Broadcast(data);
            }

            protected override Task OnReconnected(IRequest request, string connectionId)
            {
                Log.WriteLine(connectionId + " reconnected");
                return base.OnReconnected(request, connectionId);
            }

            protected override Task OnDisconnected(IRequest request, string connectionId)
            {
                Log.WriteLine(connectionId + " left");
                return base.OnDisconnected(request, connectionId);
            }

        }
    }
}
