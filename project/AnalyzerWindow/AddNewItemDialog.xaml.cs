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
using Analysis;

namespace AnalyzerWindow
{
    /// <summary>
    /// Interaction logic for AddNewItemDialog.xaml
    /// </summary>
    public partial class AddNewItemDialog : Window
    {
        public SongData SongData { get; private set; }
        
        public AddNewItemDialog()
        {
            InitializeComponent();
            SongData = new SongData();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            SongData = new SongData()
            {
                Name = TxbName.Text,
                Source = TxbSource.Text,
                Context = TxbContext.Text,
                Theme = TxbTheme.Text
            };
            DialogResult = true;
            Close();
        }
    }
}
