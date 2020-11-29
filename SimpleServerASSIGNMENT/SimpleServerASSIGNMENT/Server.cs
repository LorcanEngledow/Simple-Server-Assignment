using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace Server
{
    class Server
    {
        private ConcurrentDictionary<int, Client> clientDict;
        private TcpListener tcpListener;
        public Server(string IPaddressStr, int port)
        {
            IPAddress IPaddress = IPAddress.Parse(IPaddressStr);
            tcpListener = new TcpListener(IPaddress, port);

        }

        public void Start()
        {
            tcpListener.Start();
            clientDict = new ConcurrentDictionary<int, Client>();
            int clientIndex = 0;


            while (true) // PLACE LOGIC TO LIMIT TOTAL CONNECTIONS HERE
            {
                Socket socket = tcpListener.AcceptSocket();
                Console.WriteLine("Accepted Socket");

                int index = clientIndex;
                clientIndex++;

                Client client = new Client(socket);
                clientDict.TryAdd(index, client);

                Thread thread = new Thread(() => { ClientMethod(index); });
                thread.Start();
            }
        }
        public void Stop()
        {
            tcpListener.Stop();
        }

        private void ClientMethod(int index)
        {
            string recievedMessage;
            clientDict[index].Write("Connected to the server");

            while ((recievedMessage = clientDict[index].Read()) != null)
            {
                string response = GetReturnMessages(recievedMessage);

                if (response == "End")
                {
                    break;
                }

                clientDict[index].Write(response);
            }

            clientDict[index].Close();
            Client c;
            clientDict.TryRemove(index, out c);
        }

        private string GetReturnMessages(string code)
        {
            if (code == "Hello" || code == "Hi")
                return ("Hello!");
            else if (code == "!help")
                return ("Valid inputs are Hello,Test,End,!help");
            else if (code == "Test")
                return ("Output");
            else if (code == "End")
                return ("End");
            else
                return ("Unknown command given, !help for valid commands");
        }
    }
}
