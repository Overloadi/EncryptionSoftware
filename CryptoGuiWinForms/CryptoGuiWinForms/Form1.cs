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
    /// <summary>
    /// Software, that encrypts the contents of a file and saves the encrypted text to a different file, called "ENCRYPTED<filename>". The software has three
    /// algorithms, that can be used to encrypt: AES, TripleDES and RC2. There is also an option to transfer a file to the EncryptionSoftware server.
    /// </summary>
    public partial class Form1 : Form
    {
        public static string staticKey = "irvhjklqvbytdjkpdksnh";
        public static byte[] staticIV = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        public static string fileName;
        public static byte[] keyArrayAES;
        public static byte[] keyArrayRC2;
        public static byte[] keyArrayTDES;
        public Form1()
        {
            InitializeComponent();

            // Calculate hashes for the key with SHA512 and trim it to fit the algorithms
            SHA512CryptoServiceProvider hash2 = new SHA512CryptoServiceProvider();
            keyArrayRC2 = hash2.ComputeHash(UTF8Encoding.UTF8.GetBytes(staticKey));
            byte[] trimmedBytes2 = new byte[8];
            Buffer.BlockCopy(keyArrayRC2, 0, trimmedBytes2, 0, 8);
            keyArrayRC2 = trimmedBytes2;

            SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            keyArrayTDES = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(staticKey));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArrayTDES, 0, trimmedBytes, 0, 24);
            keyArrayTDES = trimmedBytes;

            SHA512CryptoServiceProvider hash3 = new SHA512CryptoServiceProvider();
            keyArrayAES = hash3.ComputeHash(UTF8Encoding.UTF8.GetBytes(staticKey));
            byte[] trimmedBytes3 = new byte[24];
            Buffer.BlockCopy(keyArrayAES, 0, trimmedBytes3, 0, 24);
            keyArrayAES = trimmedBytes;

           
        }

        /// <summary>
        /// Read the content of a file and send it to the server.
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
        /// Encrypt text from a file to another file with the RC2 algorithm
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="KeyArray">Key, which is converted to an byte array</param>
        /// <param name="IV">Initialization vector in an byte array</param>
        public static string EncryptRC2(string fileName, byte[] KeyArray, byte[] IV)
        {
            string path = Directory.GetCurrentDirectory() + "\\" + fileName;
            try
            {
                // byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                string plainText = File.ReadAllText(path);
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                RC2 myRC2 = RC2.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myRC2.CreateEncryptor(keyArrayRC2, staticIV), CryptoStreamMode.Write);
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
                // byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                // fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.Open);
                RC2 myRC2 = RC2.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myRC2.CreateDecryptor(keyArrayRC2, staticIV), CryptoStreamMode.Read);
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
                // byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7};
                string path = Directory.GetCurrentDirectory() + "\\" + fileName;
                string plainText = File.ReadAllText(path);
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                TripleDES myTDES = TripleDES.Create();

                CryptoStream cryptoStream = new CryptoStream(fileStream, myTDES.CreateEncryptor(keyArrayTDES, staticIV), CryptoStreamMode.Write);
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
                // byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7};
                // fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.Open);
                TripleDES myTDES = TripleDES.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myTDES.CreateDecryptor(keyArrayTDES, staticIV), CryptoStreamMode.Read);
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
                
                string plainText = File.ReadAllText(path);
                fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                Aes myAES = Aes.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myAES.CreateEncryptor(keyArrayAES, staticIV), CryptoStreamMode.Write);
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
                // byte[] IVTESTI = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,13,14,15 };
                // fileName = "ENCRYPTED" + fileName;
                FileStream fileStream = File.Open(fileName, FileMode.Open);
                Aes myAES = Aes.Create();
                CryptoStream cryptoStream = new CryptoStream(fileStream, myAES.CreateDecryptor(keyArrayAES, staticIV), CryptoStreamMode.Read);
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
        /// Browse for a file from the computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_file_Click(object sender, EventArgs e)
        {

            string fileNameTemp = "";
            DialogResult result = openFileDialog1.ShowDialog(); // Show File Dialog
            if (result == DialogResult.OK)
            {
                
                fileNameTemp = openFileDialog1.FileName;
                file_label.Text = fileNameTemp;

            }
            // Get the filename from the end of the string
            int index = fileNameTemp.LastIndexOf("\\");
            string value = fileNameTemp.Substring(index, fileNameTemp.Length - index);
            value = value.Substring(1, value.Length - 1);
            fileName = value;

        }

        /// <summary>
        /// Decrypt the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void decrypt_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                string temp = DecryptAES(fileName, keyArrayAES, staticIV);
                textBox1.Text = temp;
            }

            if (comboBox1.SelectedIndex == 1)
            {
                string tempRC2 = DecryptRC2(fileName, keyArrayRC2, staticIV);
                textBox1.Text = tempRC2;
            }
            if (comboBox1.SelectedIndex == 2)
            {
                string tempTDES = DecryptTripleDes(fileName, keyArrayTDES, staticIV);
                textBox1.Text = tempTDES;
            }
            
        }
        /// <summary>
        /// Encrypt the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                using (Aes myAes = Aes.Create())
                {
                    string fileN = EncryptAES(fileName, keyArrayAES, myAes.IV);
                    byte[] tempbyte = File.ReadAllBytes(fileN);
                    textBox1.Text = Encoding.UTF8.GetString(tempbyte);
                }
                
            }
           if (comboBox1.SelectedIndex == 1)
            {
                using (RC2 myRC2 = RC2.Create())
                {                 
                    string fileN2 = EncryptRC2(fileName, keyArrayRC2, staticIV);
                    byte[] tempbyte2 = File.ReadAllBytes(fileN2);
                    textBox1.Text = Encoding.UTF8.GetString(tempbyte2);
                }

            }
           if (comboBox1.SelectedIndex == 2)
            {
                using (TripleDES myDes = TripleDES.Create())
                {
                    keyArrayTDES = myDes.Key;
                    byte[] IVTDES = { 0, 1, 2, 3, 4, 5, 6, 7};
                    // IVTDES = myDes.IV;
                    
                    string fileN3 = EncryptTripleDes(fileName, keyArrayTDES, staticIV);
                    byte[] tempbyte3 = File.ReadAllBytes(fileN3);
                    textBox1.Text = Encoding.UTF8.GetString(tempbyte3);

                }
            }
           
        }

        /// <summary>
        /// Send a file to the server, ip address and port are acquired from the textboxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_sendFile_Click(object sender, EventArgs e)
        {
            string ip_address = textBox_ip_address.Text;
            
            try
            {
                int port = int.Parse(textBox_tcp_port.Text);
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
            catch (SocketException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (FormatException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
