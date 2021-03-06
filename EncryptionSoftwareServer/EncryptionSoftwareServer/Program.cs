﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Server for the encryption software. Listens to port 8888 and accepts clients from any IP address. When a client connects, the server reads the message in chunks
/// and writes the content of the message to a file.
/// </summary>
namespace EncryptionSoftwareServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "received.txt";

            // Create and start server
            int port = 8888;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine("Listening to port: " + port);
            Byte[] bytes = new Byte[256];
            Socket client = server.AcceptSocket();
            Console.WriteLine("Client connected");
            int bytesRead = 0;

            // Read incoming data
            using (NetworkStream ns = new NetworkStream(client))
            {
                int messageSize = 256;
                do
                {
                    byte[] chunks = new byte[messageSize];
                    bytesRead = ns.Read(bytes, 0, chunks.Length);
                }
                while (bytesRead != 0); 
              
            }
            int y = bytes.Length - 1;
            while (bytes[y] == 0)
                --y;
            byte[] dataToSave = new byte[y + 1];
            Array.Copy(bytes, dataToSave, y + 1);
            Console.WriteLine("Message received!");
            FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Write(dataToSave, 0, dataToSave.Length);
            fileStream.Close();
            client.Close();
            server.Stop();
        }
    }
}
