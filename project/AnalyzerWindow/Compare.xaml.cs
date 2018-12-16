using Microsoft.Win32;
using MusicAnalysis;
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
using System.Windows.Shapes;

namespace AnalyzerWindow
{
    /// <summary>
    /// Interaction logic for Compare.xaml
    /// </summary>
    public partial class Compare : Window
    {
        MusicAnalyzer _analyzer;

        public Compare(MusicAnalyzer analyzer)
        {
            InitializeComponent();

            _analyzer = analyzer;
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Multiselect = false,
                Filter = "WAV Files (*.wav)|*.wav|All Files (*.*)|*.*",
                Title = "Open Song File"
            };
            bool? result = openFileDialog.ShowDialog(this);

            if (result == true)
            {
                string fileName = openFileDialog.FileName;
                LblFileName.Content = fileName;
                AudioEngineCSharp.AudioEngine.Audio audio = AudioEngineCSharp.AudioEngine.Load(fileName, false, false);
                unsafe
                {
                    var testResult = _analyzer.TestData(audio.RawData, audio.DataSize);
                    LsbPercents.ItemsSource = testResult.Select((pair) => string.Format("{0} - {1}%", pair.Key, pair.Value * 100));
                }

                audio.Dispose();
            }

            
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
