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
        private ConcurrentDictionary<int, string> nameDict;
        private TcpListener tcpListener;
        private string[] baseNick1 = {"Red", "Orange", "Yellow", "Lime", "Green", "Blue", "Indigo", "Violet", "Grey"};
        private string[] baseNick2 = {"Apple", "Bunny", "Carrot", "Dog", "Elephant", "Frog", "Gourd"};
        private string userName;

        public Server(string IPaddressStr, int port)
        {
            IPAddress IPaddress = IPAddress.Parse(IPaddressStr);
            tcpListener = new TcpListener(IPaddress, port);
        }

        public void Start()
        {
            tcpListener.Start();
            clientDict = new ConcurrentDictionary<int, Client>();
            nameDict = new ConcurrentDictionary<int, string>();
            int clientIndex = 0;

            while (true) // PLACE LOGIC TO LIMIT TOTAL CONNECTIONS HERE
            {
                Socket socket = tcpListener.AcceptSocket();
                Console.WriteLine("Accepted Socket");

                int index = clientIndex;
                clientIndex++;

                Client client = new Client(socket);
                clientDict.TryAdd(index, client);

                for(int i = 0; i < 3; i++)
                {
                    Random r = new Random();
                    if (i == 0)
                    {
                        int rInt = r.Next(0, baseNick1.Length - 1);
                        userName = baseNick1[rInt];
                    }
                    else if (i == 1)
                    {
                        int rInt = r.Next(0, baseNick2.Length - 1);
                        userName += baseNick2[rInt];
                    }
                    else if (i == 2)
                    {
                        int rInt = r.Next(0, 99);
                        userName += rInt;
                        nameDict.TryAdd(index, userName);
                    }
                }

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
            string recievedMessage = "";
            clientDict[index].Write("Connected to the server");

            for (int i = 0; i <= clientDict.Count - 1; i++)
            {
                if(i != index)
                    clientDict[i].Write(nameDict[index] + " Connected");
            }

            while ((recievedMessage = clientDict[index].Read()) != null)
            {
                int ind = clientDict.Count - 1;
                string response;

                //PROCESS COMMAND INPUTS
                if (recievedMessage != "")
                {
                    if (recievedMessage[0] == '!')
                    {
                        response = GetReturnMessages(recievedMessage, index);
                        clientDict[index].Write(response);
                        if (response == "End")
                        {
                            break;
                        }
                    }
                    else
                    {
                        //SEND MESSAGES TO ALL CONNECTED CLIENTS
                        for (int i = 0; i <= ind; i++)
                        {
                            clientDict[i].Write(nameDict[index] + ": " + recievedMessage);
                        }
                    }
                }
            }
            clientDict[index].Close();
            Client c;
            clientDict.TryRemove(index, out c);
        }

        private string GetReturnMessages(string code, int ind)
        {
            switch(code)
            {
                case "!Hello":
                    return ("Hello!");
                case "!Help":
                    return ("Valid inputs are Hello,Test,End,Help");
                case "!Test":
                    return ("Output");
                case "!End":
                    return ("End");
                case "!nick":
                    return ChangeName(ind);
                default:
                    return ("Unknown command " + code + " given, !Help for valid commands");
            }
        }

        private string ChangeName(int ind)
        {
            string input;

            clientDict[ind].Write("Enter a new nickname to be used. Enter !Back to return without changing nickname");

            if((input = clientDict[ind].Read()) == "!Back")
            {
                return ("Name change cancelled");
            }
            else
            {
                nameDict[ind] = input;
                return ("Name changed to " + input);
            }
        }
    }
}
