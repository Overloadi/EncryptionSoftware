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
            Console.WriteLine(Directory.GetCurrentDirectory());
            string data = "hellohellohellohello";
            string key = "irvhjklqvbytdjkpdksnh";
            string fileName = "test.txt";
            byte[] keyArray;
            // calculate sha512 hash from key and trim it to 192 bits
            SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            keyArray = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArray, 0, trimmedBytes, 0, 24);
            keyArray = trimmedBytes;

            // rc2 test
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
                string decryptedRC2 = DecryptRC2(fileName, keyArray2, IV);
                Console.WriteLine(decryptedRC2);
            }

            // tripledes test
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

            // aes test
            using (Aes myAes = Aes.Create())
            {
                fileName = "aes.txt";
                Console.WriteLine("AES");
                Console.WriteLine("-------------");
                byte[] encrypted = EncryptStringToBytes_Aes(data, myAes.Key, myAes.IV);

                string decrypted = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);

                Console.WriteLine("Data: " + data);
                Console.WriteLine("Key : " + myAes.Key.ToString());
                Console.WriteLine("Encrypted: " + encrypted.ToString());
                Console.WriteLine("Decrypted: " + decrypted);

            } 
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
            string path = Directory.GetCurrentDirectory() + "\\" + fileName;
            string plainText = File.ReadAllText(path);
            fileName = "ENCRYPTED" + fileName;
            FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            
            // string data = File.ReadAllText(path);
            // byte[] keyArray;
            // byte[] dataArray = Convert.FromBase64String(data);

            /// keyArray = UTF8Encoding.UTF8.GetBytes(key);
            // TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            TripleDES myTDES = TripleDES.Create();
            //set the secret key for the tripleDES algorithm
            // myDES.Key = keyArray;
            // myDES.IV = IV;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            // myDES.Mode = CipherMode.CBC;
            //padding mode(if any extra byte added)
            // myDES.Padding = PaddingMode.Zeros;
            CryptoStream cryptoStream = new CryptoStream(fileStream, myTDES.CreateEncryptor(keyArray, IV), CryptoStreamMode.Write);
            StreamWriter streamWriter = new StreamWriter(cryptoStream);
            streamWriter.WriteLine(plainText);
            streamWriter.Close();
            cryptoStream.Close();
            fileStream.Close();
            // ICryptoTransform cTransform = tdes.CreateEncryptor(tdes.Key, tdes.IV);
            //transform the specified region of bytes array to resultArray
            //byte[] resultArray =
            //  cTransform.TransformFinalBlock(dataArray, 0,
            //  dataArray.Length);
            //Release resources held by TripleDes Encryptor
            //tdes.Clear();
            //Return the encrypted data into unreadable string format
            // path = Directory.GetCurrentDirectory() + "\\" + "ENCRYPTED" + fileName;
           
            // File.WriteAllBytes(path,resultArray);
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
            // byte[] keyArray;ccc
            string plainText = "";
            // string path = Directory.GetCurrentDirectory() + "\\" + fileName;
            fileName = "ENCRYPTED" + fileName;
            FileStream fileStream = File.Open(fileName, FileMode.Open);
            TripleDES myTDES = TripleDES.Create();
            CryptoStream cryptoStream = new CryptoStream(fileStream, myTDES.CreateDecryptor(keyArray, IV), CryptoStreamMode.Read);
            StreamReader streamReader = new StreamReader(cryptoStream);
            plainText = streamReader.ReadLine();
            streamReader.Close();
            cryptoStream.Close();
            fileStream.Close();
            return plainText;
            // byte[] dataArray = File.ReadAllBytes(path);

            /*SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            keyArray = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArray, 0, trimmedBytes, 0, 24);
            keyArray = trimmedBytes; */
            
            
            // TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            //tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            //tdes.Mode = CipherMode.CBC;
            //padding mode(if any extra byte added)
            //tdes.Padding = PaddingMode.Zeros;
            //tdes.Key = keyArray;
            //tdes.IV = IV;
            //ICryptoTransform cTransform = tdes.CreateDecryptor(tdes.Key, tdes.IV);
            //byte[] resultArray = cTransform.TransformFinalBlock(
            //                     dataArray, 0, dataArray.Length);
            //Release resources held by TripleDes Encryptor                
            //tdes.Clear();
            //return the Clear decrypted TEXT
            //return resultArray;
        }

        /// <summary>
        /// Encrypt plain text with AES algorithm
        /// </summary>
        /// <param name="plainText">Plaintext</param>
        /// <param name="keyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">Initialization vector in an byte array</param>
        /// <returns>Encrypted text</returns>
        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] keyArray, byte[] IV)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyArray;
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

        /// <summary>
        /// Decrypt text encrypted with AES
        /// </summary>
        /// <param name="cipherText">Cipher text</param>
        /// <param name="keyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">Initialization vector in an byte array</param>
        /// <returns>Plain text</returns>
        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] keyArray, byte[] IV)
        {
            string plaintext = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyArray;
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
