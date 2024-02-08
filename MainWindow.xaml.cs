using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace GammaDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private string? sourceFilePath = null;
        private string toEncode;
		// Selecting file for encoding/decoding
        private void ChooseFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                sourceFilePath = openFileDialog.FileName;
                toEncode = File.ReadAllText(sourceFilePath);
            }               
        }

        private void Gamma_Btn(object sender, RoutedEventArgs e)
        {
            if(sourceFilePath == null)
            {
                MessageBox.Show("No file for encoding was selected!");
                return;
            }
            if(!ulong.TryParse(SeedTextBox.Text, out ulong seed))
            {
                MessageBox.Show("Random number generator seed must be non-negative integer in range from 0 to 18 446 744 073 709 551 615");
                return;
            }
			// Path for encoded/decoded file
            string destpath = System.IO.Path.GetDirectoryName(sourceFilePath) + "\\" + System.IO.Path.GetFileNameWithoutExtension(sourceFilePath) + "(encoded).txt";
            FileStream fs = File.Create(destpath);
            fs.Close();
            StringBinaryEncoder encoder = new StringBinaryEncoder();
            string encoded;
            IHashGenerator hashGenerator;
			// Type of random number generator
            if (LCGRadio.IsChecked.Value == true)
            {
                hashGenerator = new LCGHash();                
            }
            else
            {
                hashGenerator = new XORHash();
            }
            encoded = encoder.Encode64UTF8String(toEncode, seed, hashGenerator);
            File.WriteAllText(destpath, encoded, Encoding.UTF8);
        }       
    }
}
