using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Configuration;

/// <summary>
/// Software, that encrypts the contents of a file and saves the encrypted text to a different file, called "ENCRYPTED<filename>". The software has three
/// algorithms, that can be used to encrypt: AES, TripleDES and RC2. There is also an option to transfer a file to the EncryptionSoftware server.
/// </summary>
namespace EncryptionSoftware
{
    class Program
    {
        static void Main(string[] args)
        { 
            Console.WriteLine(Directory.GetCurrentDirectory());
            string data = "hellohellohellohello";
            
            string key = "irvhjklqvbytdjkpdksnh";
            string fileName = "test.txt";
            byte[] keyArray;

            // Calculate hash for the key with SHA-512
            SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            keyArray = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArray, 0, trimmedBytes, 0, 24);
            keyArray = trimmedBytes;

            // RC2 Encrypt and decrypt test
            using (RC2 myRC2 = RC2.Create())
            {
                fileName = "rc2.txt";
                Console.WriteLine("RC2");
                Console.WriteLine("-------------");
                byte[] keyArray2;
                SHA512CryptoServiceProvider hash2 = new SHA512CryptoServiceProvider();
                keyArray2 = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                byte[] trimmedBytes2 = new byte[8];
                Buffer.BlockCopy(keyArray2, 0, trimmedBytes, 0, 8);
                keyArray2 = trimmedBytes2;

                byte[] IV = myRC2.IV;
                EncryptRC2(fileName, keyArray2, IV);
                sendFile("ENCRYPTEDrc2.txt");
                string decryptedRC2 = DecryptRC2(fileName, keyArray2, IV);
                Console.WriteLine(decryptedRC2);
            }

            // TripleDES Encrypt and decrypt test
            using (TripleDES myDes = TripleDES.Create())
            {
                fileName = "tripledes.txt";
                Console.WriteLine("TRIPLEDES");
                Console.WriteLine("-------------");
                byte[] IV = myDes.IV;
                EncryptTripleDes(fileName, keyArray, IV);
                string decrypted = DecryptTripleDes(fileName, keyArray, IV);
                Console.WriteLine(decrypted);
            }

            // AES Encrypt and decrypt test
            using (Aes myAes = Aes.Create())
            {
                fileName = "aes.txt";
                Console.WriteLine("AES");
                Console.WriteLine("-------------");
                EncryptAES(fileName, myAes.Key, myAes.IV);

                string decrypted = DecryptAES(fileName, myAes.Key, myAes.IV);

                Console.WriteLine("Decrypted: " + decrypted);

            } 
        }

        /// <summary>
        /// Read the content of a file and send it to the server, which is now located at the same computer
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        public static void sendFile(string fileName)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + fileName;
            TcpClient client = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 8888);
            client.Connect(serverEndPoint);
            NetworkStream ns = client.GetStream();
            
            byte[] fileData = File.ReadAllBytes(path);
            ns.Write(fileData, 0, fileData.Length);
            Console.WriteLine("Sent the data from file");
            
            client.Close();
            ns.Close();
        }

        /// <summary>
        /// Encrypt plaintext to a file with the RC2 algorithm
        /// </summary>
        /// <param name="plainText">Plaintext</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="KeyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">Initialization vector in an byte array</param>
        public static void EncryptRC2(string fileName, byte[] KeyArray, byte[] IV)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + fileName;
            try
            {
                string plainText = File.ReadAllText(path);
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                RC2 myRC2 = RC2.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myRC2.CreateEncryptor(KeyArray, IV), CryptoStreamMode.Write);
                StreamWriter streamWriter = new StreamWriter(cryptoStream);
                streamWriter.WriteLine(plainText);
                streamWriter.Close();
                cryptoStream.Close();
                fileStream.Close();
            }
            catch (CryptographicException cryptoException)
            {
                Console.WriteLine(cryptoException.Message);
            }
            catch (UnauthorizedAccessException fileException)
            {
                Console.WriteLine(fileException.Message);
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException.Message);
            }
        }

        /// <summary>
        /// Decrypt a file that is encrypted with RC2
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="KeyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">Initialization vector in an byte array</param>
        /// <returns>Plain text</returns>
        public static string DecryptRC2(string fileName, byte[] KeyArray, byte[] IV)
        {
            string plainText = "";
            try
            {
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.Open);
                RC2 myRC2 = RC2.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myRC2.CreateDecryptor(KeyArray, IV), CryptoStreamMode.Read);
                StreamReader streamReader = new StreamReader(cryptoStream);
                plainText = streamReader.ReadLine();
                streamReader.Close();
                cryptoStream.Close();
                fileStream.Close();
                
            }
            catch (CryptographicException cryptoException)
            {
                Console.WriteLine(cryptoException.Message);
            }
            catch (UnauthorizedAccessException fileException)
            {
                Console.WriteLine(fileException.Message);
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException.Message);
            }
            return plainText;
        }

        /// <summary>
        /// Encrypt plaintext from file with Triple DES to a file
        /// </summary>
        /// <param name="fileName">Name of the file, which is going to be encrypted</param>
        /// <param name="keyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">Initialization vector in an byte array</param>
        public static void EncryptTripleDes(string fileName, byte[] keyArray, byte[] IV)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "\\" + fileName;
                string plainText = File.ReadAllText(path);
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                TripleDES myTDES = TripleDES.Create();

                CryptoStream cryptoStream = new CryptoStream(fileStream, myTDES.CreateEncryptor(keyArray, IV), CryptoStreamMode.Write);
                StreamWriter streamWriter = new StreamWriter(cryptoStream);
                streamWriter.WriteLine(plainText);

                streamWriter.Close();
                cryptoStream.Close();
                fileStream.Close();
            }
            catch (CryptographicException cryptoException)
            {
                Console.WriteLine(cryptoException.Message);
            }
            catch (UnauthorizedAccessException fileException)
            {
                Console.WriteLine(fileException.Message);
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException.Message);
            }

        }

        /// <summary>
        /// Decrypt a file that was encrypted with DES
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="keyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">Initialization vector in an byte array</param>
        /// <returns>Plain text</returns>
        public static string DecryptTripleDes(string fileName, byte[] keyArray, byte[] IV)
        {
            string plainText = "";
            try
            {
                
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.Open);
                TripleDES myTDES = TripleDES.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myTDES.CreateDecryptor(keyArray, IV), CryptoStreamMode.Read);
                StreamReader streamReader = new StreamReader(cryptoStream);
                plainText = streamReader.ReadLine();
                streamReader.Close();
                cryptoStream.Close();
                fileStream.Close();
            }

            catch (CryptographicException cryptoException)
            {
                Console.WriteLine(cryptoException.Message);
            }
            catch (UnauthorizedAccessException fileException)
            {
                Console.WriteLine(fileException.Message);
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException.Message);
            }

            return plainText;
        }

        /// <summary>
        /// Encrypt plain text with AES algorithm and save it to a file
        /// </summary>
        /// <param name="plainText">Plaintext</param>
        /// <param name="keyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">Initialization vector in an byte array</param>
        public static void EncryptAES(string fileName, byte[] keyArray, byte[] IV)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + fileName;
            try
            {
                string plainText = File.ReadAllText(path);
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                Aes myAES = Aes.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myAES.CreateEncryptor(keyArray, IV), CryptoStreamMode.Write);
                StreamWriter streamWriter = new StreamWriter(cryptoStream);
                streamWriter.WriteLine(plainText);
                streamWriter.Close();
                cryptoStream.Close();
                fileStream.Close();
            }
            catch (CryptographicException cryptoException)
            {
                Console.WriteLine(cryptoException.Message);
            }
            catch (UnauthorizedAccessException fileException)
            {
                Console.WriteLine(fileException.Message);
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException.Message);
            }

        }

        /// <summary>
        /// Decrypt text encrypted with AES in a file
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="keyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">Initialization vector in an byte array</param>
        /// <returns>Plain text</returns>
        public static string DecryptAES(string fileName, byte[] keyArray, byte[] IV)
        {
            string plainText = "";
            try
            {
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.Open);
                Aes myAES = Aes.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myAES.CreateDecryptor(keyArray, IV), CryptoStreamMode.Read);
                StreamReader streamReader = new StreamReader(cryptoStream);
                plainText = streamReader.ReadLine();
                streamReader.Close();
                cryptoStream.Close();
                fileStream.Close();
            }
            catch (CryptographicException cryptoException)
            {
                Console.WriteLine(cryptoException.Message);
            }
            catch (UnauthorizedAccessException fileException)
            {
                Console.WriteLine(fileException.Message);
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException.Message);
            }
            return plainText;

        }
    }
}
