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

namespace Crypto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String inputfilepath;
        private String inputname;
        private String outputfilepath;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InputSelect_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.ShowDialog();
            var filepath = dlg.FileName;
            inputfilepath = filepath;
            inputname = Path.GetFileName(filepath);
            filename.Text = inputfilepath;
        }

        private void OutputFolderSelect_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory where new file should be saved" };
            dlg.ShowDialog();
            outputfilepath = dlg.SelectedPath;
        }
    }
}
