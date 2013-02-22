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
                        case ConsoleKey.G:
                            var groupContext = GlobalHost.ConnectionManager.GetConnectionContext<GroupConnection>();
                            groupContext.Groups.Send("foo", "this is for the foo-group");
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
                app.MapConnection<GroupConnection>("/group");
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

        public class GroupConnection : PersistentConnection
        {
            protected override Task OnConnected(IRequest request, string connectionId)
            {
                return Groups.Add(connectionId, "foo");
            }

            protected override Task OnReceived(IRequest request, string connectionId, string data)
            {
                // Messages are sent with the following format
                // group:message
                string[] decoded = data.Split(':');
                string groupName = decoded[0];
                string message = decoded[1];

                // Send a message to the specified
                return Groups.Send(groupName, message);
            }

            protected override Task OnDisconnected(IRequest request, string connectionId)
            {
                return Groups.Remove(connectionId, "foo");
            }

            protected override IList<string> OnRejoiningGroups(IRequest request, IList<string> groups, string connectionId)
            {
                return groups;
            }
        }
    }
}
