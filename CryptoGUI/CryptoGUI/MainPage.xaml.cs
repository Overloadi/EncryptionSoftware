using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CryptoGUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string selected;


        public MainPage()
        {
            this.InitializeComponent();
            selected = "";
        }

        private void button_aes_Click(object sender, RoutedEventArgs e)
        {
            selected_algo.Text = "AES";

        }

        private void button_3des_Click(object sender, RoutedEventArgs e)
        {
            selected_algo.Text = "3DES";
        }

        private void button_rc2_Click(object sender, RoutedEventArgs e)
        {
            selected_algo.Text = "RC2";
        }

        private void encrypt_click(object sender, RoutedEventArgs e)
        {
            string data = "moi";
        
                Encrypt(selected,data);
        }
        private void Encrypt(string algo,string data)
        {
            
            byte[] EncryptStringToBytes_Aes(string plainText, byte[] keyArray, byte[] IV)
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
            using (Aes myAes = Aes.Create())
            {
   
                byte[] encrypted = EncryptStringToBytes_Aes(data, myAes.Key, myAes.IV);
                Debug.WriteLine("Key : " + myAes.Key.ToString());
                Debug.WriteLine("Encrypted: " + encrypted.ToString());
              

            }
        }


    }
}
