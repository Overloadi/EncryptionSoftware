using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionSoftwareServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "received.txt";
            int j = 0;
            
            TcpListener server = new TcpListener(IPAddress.Any, 8888);
            server.Start();
            Byte[] bytes = new Byte[256];
            string data = null;
            while (true)
            {
                fileName = j + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                Console.WriteLine("Waiting for connection");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected");
                data = null;
                NetworkStream stream = client.GetStream();
                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes("Kuitti");

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", data);
                }

                // Shutdown and end connection
                client.Close();
                fileStream.Close();
                j++;
            }
        }
    }
}
