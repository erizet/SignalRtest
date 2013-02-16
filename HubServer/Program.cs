using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.AspNet.SignalR;

namespace HubServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://*:8082";

            using (WebApplication.Start<Startup>(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // This will map out to http://localhost:8080/signalr by default
            app.MapHubs();
        }
    }

    public class Chat : Hub
    {
        public void Send(string message)
        {
            // Call the addMessage method on all clients            
            Clients.All.addMessage(message);
        }
    }
}
