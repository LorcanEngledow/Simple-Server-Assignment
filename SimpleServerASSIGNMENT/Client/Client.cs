using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Client
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;
        private ClientForm clientForm;

        public Client()
        {
            tcpClient = new TcpClient();
        }

        public bool Connect(String ipAddress, int port)
        {
            IPAddress IPAddress = IPAddress.Parse(ipAddress);

            try
            {
                tcpClient.Connect(IPAddress, port);

                stream = tcpClient.GetStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream);

                return (true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return (false);
            }
        }

        public void Run()
        {
            string userInput;
            ProcessServerResponse();

            clientForm = new ClientForm(this);

            while ((userInput = Console.ReadLine()) != null)
            {
                writer.WriteLine(userInput);
                writer.Flush();

                ProcessServerResponse();

                if (userInput == "End")
                    break;
            }

            tcpClient.Close();
        }

        private void ProcessServerResponse()
        {
            writer.WriteLine("Server response: " + reader.ReadLine());
            writer.WriteLine();
        }

        public void SendMessage(string message)
        {

        }
    }
}
