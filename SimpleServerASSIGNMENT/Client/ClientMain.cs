using System;
using System.Runtime.CompilerServices;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Client
{
    class ClientMain
    {
        [STAThread]
        static void Main(string[] args)
        {
            Client client = new Client();
            if (client.Connect("127.0.0.1", 4444))
            {
                Console.WriteLine("Successfully connected");
                client.Run();
            }
            else
                Console.WriteLine("Connection Failed");
        }
    }
}