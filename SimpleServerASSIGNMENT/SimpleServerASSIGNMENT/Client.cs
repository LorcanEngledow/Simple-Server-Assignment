using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Server
{
    class Client
    {
        private Socket m_Socket;
        private NetworkStream m_Stream;
        private StreamReader m_Reader;
        private StreamWriter m_Writer;

        private object m_ReadLock;
        private object m_WriteLock;

        public Client(Socket socket)
        {
            m_WriteLock = new object();
            m_ReadLock = new object();
            m_Socket = socket;

            m_Stream = new NetworkStream(m_Socket);

            m_Reader = new StreamReader(m_Stream, Encoding.UTF8);
            m_Writer = new StreamWriter(m_Stream);
        }

        public void Close()
        {
            m_Stream.Close();
            m_Reader.Close();
            m_Writer.Close();
            m_Socket.Close();
        }

        public string Read()
        {
            lock (m_ReadLock)
            {
                return (m_Reader.ReadLine());
            }
        }

        public void Write(string message)
        {
            lock (m_WriteLock)
            {
                m_Writer.WriteLine(message);
                m_Writer.Flush();
            }
        }
    }
}
