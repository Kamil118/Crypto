using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using Org.BouncyCastle;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace Crypto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    ///https://stackoverflow.com/questions/49865042/c-sharp-extract-public-key-from-rsa-pem-private-key
    public partial class MainWindow : Window
    {
        private String inputfilepath = null;
        private String inputname = null;
        private String outputfilepath = null;
        private String public_key = null;
        private String private_key = null;
        private String checksum = null;
        public MainWindow()
        {
            InitializeComponent();
        }


        private static bool validateKey(String key)
        {
            if (key != null && key != "")
            {
                return true;
            }
            return false;
        }
        private void update_buttons()
        {
            if(validateKey(private_key))
            {
                //generateButton.IsEnabled = true; not implemented yet
            }
            else
            {
                generateButton.IsEnabled = false;
            }

            if(validateKey(private_key) && inputfilepath != null)
            {
                decryptButton.IsEnabled = true;
            }
            else
            {
                decryptButton.IsEnabled = false;
            }

            if (validateKey(public_key) && inputfilepath != null)
            {
                encryptButton.IsEnabled = true;
            }
            else
            {
                encryptButton.IsEnabled = false;
            }
        }

        private void setInputFile(String filepath)
        {
            inputfilepath = filepath;
            inputname = Path.GetFileName(filepath);
            filename.Text = inputfilepath;
        }
        private void InputSelect_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.ShowDialog();
            var filepath = dlg.FileName;
            setInputFile(filepath);
            update_buttons();
        }

        private void setOutputDir(String path)
        {
            outputfilepath = path;
            output.Text = outputfilepath;
        }

        private void OutputFolderSelect_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            
            if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                setOutputDir(dlg.FileName);
            }
            else
            {
                setOutputDir("");
            }
            update_buttons();
        }

        private void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            FileClass fc = new FileClass(inputfilepath, outputfilepath);
            var rsakey = RSA.Create();
            var rsakey2 = new RSACryptoServiceProvider(2048);
            rsakey.ImportSubjectPublicKeyInfo(Convert.FromBase64String(public_key), out _);
            rsakey2.ImportParameters(rsakey.ExportParameters(false));
            try
            {
                fc.encrypt(rsakey2);
            }
            catch (System.IO.IOException ex)
            {
                System.Windows.Forms.MessageBox.Show("IO Error");
                fc.Dispose();
                return;
            }

            System.Windows.Forms.MessageBox.Show("Encryption Finished");
            fc.Dispose();
        }

        private void decryptButton_Click(object sender, RoutedEventArgs e)
        {
            FileClass fc = new FileClass(inputfilepath, Path.GetDirectoryName(outputfilepath)+"\\");
            var rsakey = RSA.Create();
            var rsakey2 = new RSACryptoServiceProvider(2048);
            rsakey.ImportRSAPrivateKey(Convert.FromBase64String(private_key), out _);
            rsakey2.ImportParameters(rsakey.ExportParameters(true));
            try
            {
                fc.decrypt(rsakey2, Path.GetFileName(outputfilepath));
            }
            catch (System.IO.IOException ex)
            {
                System.Windows.Forms.MessageBox.Show("IO Error");
                fc.Dispose();
                return;
            }
            
            System.Windows.Forms.MessageBox.Show("Decryption Finished");
            fc.Dispose();
        }

        private void _publicKey_TextInput(object sender, TextChangedEventArgs e)
        {
            public_key = _publicKey.Text;
            update_buttons();
        }


        private void privateKey_TextInput(object sender, TextChangedEventArgs e)
        {
            private_key = privateKey.Text;
            update_buttons();
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            var keyPair = DotNetUtilities.GetRsaKeyPair(rsa);
            var sw = new StringWriter();
            var pw = new PemWriter(sw);
            pw.WriteObject(keyPair.Private);
            privateKey.Text = sw.ToString().Replace("-----BEGIN RSA PRIVATE KEY-----\r\n","").Replace("\r\n-----END RSA PRIVATE KEY-----\r\n", "");
            sw = new StringWriter();
            pw = new PemWriter(sw);
            pw.WriteObject(keyPair.Public);
            _publicKey.Text = sw.ToString().Replace("-----BEGIN PUBLIC KEY-----\r\n", "").Replace("\r\n-----END PUBLIC KEY-----\r\n", "");
            private_key = privateKey.Text;
            public_key = _publicKey.Text;
            update_buttons();
        }
    }
}
