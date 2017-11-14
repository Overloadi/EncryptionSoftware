using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace CryptoGuiWinForms

{
    
    public partial class Form1 : Form
    {
        public static string key = "irvhjklqvbytdjkpdksnh";
        public static string fileName;
        public static byte[] keyArrayAES;
        public static byte[] IVAES;
        public static byte[] keyArrayRC2;
        public static byte[] IVRC2;
        public static byte[] IVTDES;
        public static byte[] keyArrayTDES;
        public Form1()
        {
            InitializeComponent();
            string key = "irvhjklqvbytdjkpdksnh";
            // string fileName = "test.txt";
            byte[] keyArray;
            // Calculate hash for the key with SHA-512
            SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            keyArray = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArray, 0, trimmedBytes, 0, 24);
            keyArray = trimmedBytes;

            /*
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
                //sendFile("ENCRYPTEDrc2.txt");
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

            } */
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
        public static string EncryptRC2(string fileName, byte[] KeyArray, byte[] IV)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + fileName;
            try
            {
                byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                string plainText = File.ReadAllText(path);
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                RC2 myRC2 = RC2.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myRC2.CreateEncryptor(KeyArray, IVTESTI), CryptoStreamMode.Write);
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
            return fileName;
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
                byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                // fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.Open);
                RC2 myRC2 = RC2.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myRC2.CreateDecryptor(KeyArray, IVTESTI), CryptoStreamMode.Read);
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
        public static string EncryptTripleDes(string fileName, byte[] keyArray, byte[] IV)
        {
            try
            {
                byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7};
                string path = Directory.GetCurrentDirectory() + "\\" + fileName;
                string plainText = File.ReadAllText(path);
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                TripleDES myTDES = TripleDES.Create();

                CryptoStream cryptoStream = new CryptoStream(fileStream, myTDES.CreateEncryptor(keyArray, IVTESTI), CryptoStreamMode.Write);
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
            return fileName;
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
                // Changed random IV to IV that is static all the time! 
                byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7};
                // fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.Open);
                TripleDES myTDES = TripleDES.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myTDES.CreateDecryptor(keyArray, IVTESTI), CryptoStreamMode.Read);
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
        public static string EncryptAES(string fileName, byte[] keyArray, byte[] IV)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + fileName;
            try
            {
                byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,13,14,15};
                string plainText = File.ReadAllText(path);
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                Aes myAES = Aes.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myAES.CreateEncryptor(keyArray, IVTESTI), CryptoStreamMode.Write);
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
            return fileName;
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
                byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,13,14,15 };
                // fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.Open);
                Aes myAES = Aes.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myAES.CreateDecryptor(keyArray, IVTESTI), CryptoStreamMode.Read);
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
    

        private void button_file_Click(object sender, EventArgs e)
        {

            string fileNameTemp = "";
            DialogResult result = openFileDialog1.ShowDialog(); // Show File Dialog
            if (result == DialogResult.OK)
            {
                
                fileNameTemp = openFileDialog1.FileName;
                file_label.Text = fileNameTemp;

            }
            int index = fileNameTemp.LastIndexOf("\\");
            string value = fileNameTemp.Substring(index, fileNameTemp.Length - index);
            value = value.Substring(1, value.Length - 1);
            fileName = value;

        }

        private void decrypt_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
                keyArrayAES = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                byte[] trimmedBytes = new byte[24];
                Buffer.BlockCopy(keyArrayAES, 0, trimmedBytes, 0, 24);
                keyArrayAES = trimmedBytes;
                string temp = DecryptAES(fileName, keyArrayAES, IVAES);
                textBox1.Text = temp;
            }

            if (comboBox1.SelectedIndex == 1)
            {
                SHA512CryptoServiceProvider hash2 = new SHA512CryptoServiceProvider();
                keyArrayRC2 = hash2.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                byte[] trimmedBytes2 = new byte[8];
                Buffer.BlockCopy(keyArrayRC2, 0, trimmedBytes2, 0, 8);
                keyArrayRC2 = trimmedBytes2;
                string tempRC2 = DecryptRC2(fileName, keyArrayRC2, IVRC2);
                textBox1.Text = tempRC2;
            }
            if (comboBox1.SelectedIndex == 2)
            {
                // string key = "irvhjklqvbytdjkpdksnh";
                SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
                keyArrayTDES = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                byte[] trimmedBytes = new byte[24];
                Buffer.BlockCopy(keyArrayTDES, 0, trimmedBytes, 0, 24);
                keyArrayTDES = trimmedBytes;
                string tempTDES = DecryptTripleDes(fileName, keyArrayTDES, IVTDES);
                textBox1.Text = tempTDES;
            }
            
        }
        private void encrypt_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null)
            {
                textBox1.Text = "Please select algrithm";
            }
            if (fileName == null)
            {
                textBox1.Text = "Please select file !!";
            }
        
            if (comboBox1.SelectedIndex == 0)
            {
                // string key = "irvhjklqvbytdjkpdksnh";
                // string fileName = "test.txt";
                // Calculate hash for the key with SHA-512
                SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
                keyArrayAES = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                byte[] trimmedBytes = new byte[24];
                Buffer.BlockCopy(keyArrayAES, 0, trimmedBytes, 0, 24);
                keyArrayAES = trimmedBytes;
                using (Aes myAes = Aes.Create())
                {
                    // myAes.Key = keyArrayAES;
                    // IVAES = myAes.IV;
                    string fileN = EncryptAES(fileName, keyArrayAES, myAes.IV);
                    byte[] tempbyte = File.ReadAllBytes(fileN);
                    textBox1.Text = Encoding.UTF8.GetString(tempbyte);
                }
                
            }
           if (comboBox1.SelectedIndex == 1)
            {
                // string key = "irvhjklqvbytdjkpdksnh";
                SHA512CryptoServiceProvider hash2 = new SHA512CryptoServiceProvider();
                keyArrayRC2 = hash2.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                byte[] trimmedBytes2 = new byte[8];
                Buffer.BlockCopy(keyArrayRC2, 0, trimmedBytes2, 0, 8);
                keyArrayRC2 = trimmedBytes2;
                using (RC2 myRC2 = RC2.Create())
                {
                    

                    /*SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
                    keyArrayRC2 = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    byte[] trimmedBytes = new byte[24];
                    Buffer.BlockCopy(keyArrayRC2, 0, trimmedBytes, 0, 24);
                    keyArrayRC2 = trimmedBytes; */

                    
                    // IVRC2 = myRC2.IV;
                    string fileN2 = EncryptRC2(fileName, keyArrayRC2, IVRC2);
                    byte[] tempbyte2 = File.ReadAllBytes(fileN2);
                    textBox1.Text = Encoding.UTF8.GetString(tempbyte2);
                }

            }
           if (comboBox1.SelectedIndex == 2)
            {
                // string key = "irvhjklqvbytdjkpdksnh";
                SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
                keyArrayTDES = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                byte[] trimmedBytes = new byte[24];
                Buffer.BlockCopy(keyArrayTDES, 0, trimmedBytes, 0, 24);
                keyArrayTDES = trimmedBytes; 
                using (TripleDES myDes = TripleDES.Create())
                {
                    keyArrayTDES = myDes.Key;
                    byte[] IVTDES = { 0, 1, 2, 3, 4, 5, 6, 7};
                    // IVTDES = myDes.IV;
                    
                    string fileN3 = EncryptTripleDes(fileName, keyArrayTDES, IVTDES);
                    byte[] tempbyte3 = File.ReadAllBytes(fileN3);
                    textBox1.Text = Encoding.UTF8.GetString(tempbyte3);

                }
            }
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ip_address = textBox3.Text;
            int port = int.Parse(textBox2.Text);

                string path = Directory.GetCurrentDirectory() + "\\" + fileName;
                TcpClient client = new TcpClient();
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip_address), port);
                client.Connect(serverEndPoint);
                NetworkStream ns = client.GetStream();

                byte[] fileData = File.ReadAllBytes(path);
                ns.Write(fileData, 0, fileData.Length);
                Console.WriteLine("Sent the data from file");

                client.Close();
                ns.Close();
            
        }
    }
           
}
