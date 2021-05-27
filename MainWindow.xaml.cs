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
                generateButton.IsEnabled = true;
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
            var dlg = new FolderBrowserDialog() { Description = "Select directory where new file should be saved" };
            dlg.ShowDialog();
            setOutputDir(dlg.SelectedPath);
            update_buttons();
        }

        private void encryptButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void decryptButton_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
