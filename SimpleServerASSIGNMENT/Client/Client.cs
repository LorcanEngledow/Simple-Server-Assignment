using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;


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
        [STAThread]
        public void Run()
        {
            Client client = new Client();
            clientForm = new ClientForm(this);

            Thread thread = new Thread(() => { ProcessServerResponse(); });
            thread.Start();

            clientForm.ShowDialog();
            tcpClient.Close();
        }

        private void ProcessServerResponse()
        {
            while (reader != null)
            {
                clientForm.UpdateChatWindow(reader.ReadLine());
            }
        }

        public void SendMessage(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }
    }
}
