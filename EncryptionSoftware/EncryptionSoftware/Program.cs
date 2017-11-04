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

namespace EncryptionSoftware
{
    class Program
    {
        static void Main(string[] args)
        {
            /* StreamReader sr = new StreamReader("source.txt");

            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(new IPEndPoint(IPAddress.Parse("192.168.0.3"), 5442));

            byte[] buffer = new byte[1500];
            long bytesSent = 0;

            while (bytesSent < sr.BaseStream.Length)
            {
                int bytesRead = sr.BaseStream.Read(buffer, 0, 1500);
                tcpClient.GetStream().Write(buffer, 0, bytesRead);
                Console.WriteLine(bytesRead + " bytes sent.");

                bytesSent += bytesRead;
            }

            tcpClient.Close();

            Console.WriteLine("finished");
            Console.ReadLine(); */
            string data = "hellohellohellohello";
            string key = "irvhjklqvbytdjkpdksnh";

            byte[] keyArray;
            SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            keyArray = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArray, 0, trimmedBytes, 0, 24);
            keyArray = trimmedBytes;
            using (TripleDES myDes = TripleDES.Create())
            {
                byte[] IV = myDes.IV;
                byte[] encrypted = EncryptTripleDes(data, keyArray, IV);
                Console.WriteLine(Convert.ToBase64String(encrypted, 0, encrypted.Length));
                string decrypted = DecryptTripleDes(encrypted, keyArray, IV);
                Console.WriteLine(decrypted);
            }

            using (Aes myAes = Aes.Create())
            {
                byte[] encrypted = EncryptStringToBytes_Aes(data, myAes.Key, myAes.IV);

                string decrypted = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);

                Console.WriteLine("Data: " + data);
                Console.WriteLine("Key : " + myAes.Key.ToString());
                Console.WriteLine("Encrypted: " + encrypted.ToString());
                Console.WriteLine("Decrypted: " + decrypted);

            } 
        }

        /// <summary>
        /// Encrypt plaintext with Triple DES
        /// </summary>
        /// <param name="data">Plaintext</param>
        /// <param name="keyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">initialization vector</param>
        /// <returns>Encrypted byte array</returns>
        public static byte[] EncryptTripleDes(string data, byte[] keyArray, byte[] IV)
        {
            // byte[] keyArray;
            byte[] dataArray = UTF8Encoding.UTF8.GetBytes(data);



            /// keyArray = UTF8Encoding.UTF8.GetBytes(key);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            tdes.IV = IV;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.CBC;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = tdes.CreateEncryptor(tdes.Key, tdes.IV);
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(dataArray, 0,
              dataArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return resultArray;
        }

        /// <summary>
        /// Decrypt a cipher text that was encrypted with DES
        /// </summary>
        /// <param name="cipher">Ciphertext</param>
        /// <param name="keyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">initialization vector</param>
        /// <returns>Plaintext</returns>
        public static string DecryptTripleDes(byte[] cipher, byte[] keyArray, byte[] IV)
        {
            // byte[] keyArray;
            byte[] dataArray = cipher;

            /*SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            keyArray = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArray, 0, trimmedBytes, 0, 24);
            keyArray = trimmedBytes; */
            

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            //tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.CBC;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.None;
            tdes.Key = keyArray;
            tdes.IV = IV;
            ICryptoTransform cTransform = tdes.CreateDecryptor(tdes.Key, tdes.IV);
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 dataArray, 0, dataArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }


        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
