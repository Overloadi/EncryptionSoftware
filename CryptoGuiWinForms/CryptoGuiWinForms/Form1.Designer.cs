namespace CryptoGuiWinForms
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.encrypt = new System.Windows.Forms.Button();
            this.decrypt = new System.Windows.Forms.Button();
            this.button_file = new System.Windows.Forms.Button();
            this.file_label = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button_sendFile = new System.Windows.Forms.Button();
            this.textBox_tcp_port = new System.Windows.Forms.TextBox();
            this.textBox_ip_address = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // encrypt
            // 
            this.encrypt.Location = new System.Drawing.Point(12, 64);
            this.encrypt.Name = "encrypt";
            this.encrypt.Size = new System.Drawing.Size(84, 23);
            this.encrypt.TabIndex = 0;
            this.encrypt.Text = "Encrypt";
            this.encrypt.UseVisualStyleBackColor = true;
            this.encrypt.Click += new System.EventHandler(this.encrypt_Click);
            // 
            // decrypt
            // 
            this.decrypt.Location = new System.Drawing.Point(102, 64);
            this.decrypt.Name = "decrypt";
            this.decrypt.Size = new System.Drawing.Size(84, 23);
            this.decrypt.TabIndex = 1;
            this.decrypt.Text = "Decrypt";
            this.decrypt.UseVisualStyleBackColor = true;
            this.decrypt.Click += new System.EventHandler(this.decrypt_Click);
            // 
            // button_file
            // 
            this.button_file.Location = new System.Drawing.Point(12, 35);
            this.button_file.Name = "button_file";
            this.button_file.Size = new System.Drawing.Size(84, 23);
            this.button_file.TabIndex = 5;
            this.button_file.Text = "Select file";
            this.button_file.Click += new System.EventHandler(this.button_file_Click);
            // 
            // file_label
            // 
            this.file_label.AutoSize = true;
            this.file_label.Location = new System.Drawing.Point(102, 40);
            this.file_label.Name = "file_label";
            this.file_label.Size = new System.Drawing.Size(61, 13);
            this.file_label.TabIndex = 4;
            this.file_label.Text = "chosen file:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "AES",
            "RC2",
            "3DES"});
            this.comboBox1.Location = new System.Drawing.Point(12, 8);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(84, 21);
            this.comboBox1.TabIndex = 6;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 104);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(303, 188);
            this.textBox1.TabIndex = 7;
            // 
            // button_sendFile
            // 
            this.button_sendFile.Location = new System.Drawing.Point(423, 248);
            this.button_sendFile.Name = "button_sendFile";
            this.button_sendFile.Size = new System.Drawing.Size(160, 44);
            this.button_sendFile.TabIndex = 8;
            this.button_sendFile.Text = "Send File";
            this.button_sendFile.UseVisualStyleBackColor = true;
            this.button_sendFile.Click += new System.EventHandler(this.button_sendFile_Click);
            // 
            // textBox_tcp_port
            // 
            this.textBox_tcp_port.Location = new System.Drawing.Point(423, 222);
            this.textBox_tcp_port.Name = "textBox_tcp_port";
            this.textBox_tcp_port.Size = new System.Drawing.Size(160, 20);
            this.textBox_tcp_port.TabIndex = 9;
            // 
            // textBox_ip_address
            // 
            this.textBox_ip_address.Location = new System.Drawing.Point(423, 193);
            this.textBox_ip_address.Name = "textBox_ip_address";
            this.textBox_ip_address.Size = new System.Drawing.Size(160, 20);
            this.textBox_ip_address.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(321, 196);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "IP ADDRESS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(321, 222);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "TCP PORT";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 304);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_ip_address);
            this.Controls.Add(this.textBox_tcp_port);
            this.Controls.Add(this.button_sendFile);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.file_label);
            this.Controls.Add(this.button_file);
            this.Controls.Add(this.decrypt);
            this.Controls.Add(this.encrypt);
            this.Name = "Form1";
            this.Text = "Simple File Encryption Software";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button encrypt;
        private System.Windows.Forms.Button decrypt;
        private System.Windows.Forms.Button button_file;
        private System.Windows.Forms.Label file_label;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button_sendFile;
        private System.Windows.Forms.TextBox textBox_tcp_port;
        private System.Windows.Forms.TextBox textBox_ip_address;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

