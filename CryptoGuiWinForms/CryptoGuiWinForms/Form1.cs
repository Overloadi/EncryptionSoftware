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

namespace CryptoGuiWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_file_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show File Dialog
            if (result == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                file_label.Text = file;
                string text = File.ReadAllText(file);
            }

        }

        private void decrypt_Click(object sender, EventArgs e)
        {

        }

        private void encrypt_Click(object sender, EventArgs e)
        {
            Cryptos algortimit = new Cryptos();
            algortimit.Crypto();
           
        }
    }
           
}
