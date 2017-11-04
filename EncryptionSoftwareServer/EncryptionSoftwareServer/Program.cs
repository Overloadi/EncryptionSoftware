using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EncryptionSoftwareServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(StartTCPServer);

        }
        private static void StartTCPServer(object state)
        {
            TcpListener tcpServer = new TcpListener(IPAddress.Parse("192.168.0.3"), 5442);
            tcpServer.Start();
            TcpClient client = tcpServer.AcceptTcpClient();

            Console.WriteLine("Client connection accepted from " + client.Client.RemoteEndPoint + ".");

            StreamWriter sw = new StreamWriter("destination.txt");

            byte[] buffer = new byte[1500];
            int bytesRead = 1;

            while (bytesRead > 0)
            {
                bytesRead = client.GetStream().Read(buffer, 0, 1500);

                if (bytesRead == 0)
                {
                    break;
                }

                sw.BaseStream.Write(buffer, 0, bytesRead);
                Console.WriteLine(bytesRead + " written.");
            }

            sw.Close();
        }

    }
}
