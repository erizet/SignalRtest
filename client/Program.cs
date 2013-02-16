using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNet;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace client
{
    class Program
    {
        static void Main(string[] args)
        {

            Microsoft.AspNet.SignalR.Client.Connection conn = new Microsoft.AspNet.SignalR.Client.Connection("http://ipv4.fiddler:8082/echo");
            conn.Received += data =>
            {
                Log.WriteLine(data);
            };
            conn.Error += ex =>
            {
                Log.WriteLine("An error occurred " +ex.Message);
            };
            conn.Closed += () =>
            {
                Log.WriteLine("Connection with client id " + conn.ConnectionId + " closed");
            };

            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D:
                        conn.Stop();
                        break;
                    case ConsoleKey.S:
                        conn.Send("Hello").ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Log.WriteLine("Send failed " + task.Exception.GetBaseException());
                            }
                            else
                            {
                                Log.WriteLine("Success");
                            }
                        });
                        break;
                    case ConsoleKey.U:
                        Task start = conn.Start(new LongPollingTransport(new MyHttpClient())).ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Log.WriteLine("Failed to start: " + task.Exception.GetBaseException());
                            }
                            else
                            {
                                Log.WriteLine("Success! Connected with client connection id " + conn.ConnectionId);
                                // Do more stuff here
                            }
                        });
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
}
