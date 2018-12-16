using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Accord.IO;
using Analysis;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using MusicAnalysis;
using Forms = System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace AnalyzerWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // private AnalysisHelper _analysis;
        private ExcelReader _reader;
        private MusicAnalyzer _musicAnalysis;

        private List<SongData> _data;
        private string _currentFilePath;

        private BackgroundWorker _backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();

            _backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };
            _backgroundWorker.DoWork += Worker_Work;
            _backgroundWorker.RunWorkerCompleted += Worker_RunComplete;
            _backgroundWorker.ProgressChanged += Worker_UpdateProgress;
            New();

        }


        ~MainWindow()
        {
            _backgroundWorker.Dispose();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            New();
        }


        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFilePath != null)
            {
                SaveFile();
            }
            else
            {
                SaveAsFile();
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAsFile();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddNewItemDialog newItem = new AddNewItemDialog();
            bool? result = newItem.ShowDialog();
            if (result == true)
            {
                _data.Add(newItem.SongData);

                UpdateList();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            int index = LbxSongList.SelectedIndex;
            if (index >= 0)
            {
                _data.RemoveAt(index);
                UpdateList();
            }

        }

        private void New()
        {
            _currentFilePath = null;
            _data = new List<SongData>();
            UpdateList();
        }

        private void OpenFileDialog()
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                Title = "Open file...",
                RestoreDirectory = true,
                Multiselect = false,
                Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*"
            };
            bool? result = open.ShowDialog(this);
            if (result == true) // isn't that funny...
            {
                Stream s = open.OpenFile();
                string[,] mat;
                using (CsvReader reader = new CsvReader(s, true))
                {
                    mat = reader.ToMatrix<string>();
                }
                _data = new List<SongData>();
                for (int i = 0; i < mat.GetLength(0); ++i)
                {
                    _data.Add(new SongData()
                    {
                        Name = mat[i, 0],
                        Source = mat[i, 1],
                        Context = mat[i, 2],
                        Theme = mat[i, 3]
                    });
                }
                UpdateList();
                _currentFilePath = open.FileName;
            }
        }



        private void LbxSongList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // update right contents
        }

        private void TxbEpoch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string text = TxbEpoch.Text;
            int index = TxbEpoch.CaretIndex;
            const string noNum = "\\D";
            string newString = Regex.Replace(text, noNum, "");
            TxbEpoch.Text = newString;
            if (!newString.Equals(text))
                --index;
            TxbEpoch.CaretIndex = index;
        }



        private void SaveFile()
        {
            using (CsvWriter writer = new CsvWriter(_currentFilePath))
            {
                writer.WriteHeaders("Name", "Source", "Context", "Theme");
                foreach (SongData song in _data)
                {
                    writer.WriteLine(new[]
                    {
                        song.Name, song.Source, song.Context, song.Theme
                    });
                }
            }
        }

        private void SaveAsFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Save file...",
                RestoreDirectory = true,
                OverwritePrompt = true,
                Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*"
            };
            bool? success = saveFileDialog.ShowDialog(this);
            if (success == true)
            {
                _currentFilePath = saveFileDialog.FileName;
                SaveFile();
            }

        }

        private void UpdateList()
        {
            LbxSongList.ItemsSource = _data.Select(s => $"({s.Source}) {s.Name}");
        }

        private bool _analysisTypeContextSelected = true;

        private struct MusicArg
        {
            public MusicAnalyzer Analyzer;
            public int Epoch;
            public bool IsContext;
            public List<SongData> SongData;
            public string FilePath;
            public string SavePath;
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {

            if (_data.Count <= 0)
            {
                MessageBox.Show(this, "You cannot run an analysis without input", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Forms.FolderBrowserDialog folderDialog = new Forms.FolderBrowserDialog()
            {
                Description = "Song Directory",
                ShowNewFolderButton = false,
            };
            Forms.DialogResult dialogResult = folderDialog.ShowDialog();
            if (dialogResult == Forms.DialogResult.OK)
            {
                string selectedSongPath = folderDialog.SelectedPath;
                var list = _data.Select(sd => _analysisTypeContextSelected ? sd.Context : sd.Theme);
                var uniqueList = list.Distinct().ToList();
                // put this in a new thread
                _musicAnalysis = new MusicAnalyzer(uniqueList);
                // load

                int epoch = 0;
                int.TryParse(TxbEpoch.Text, out epoch);
                object arguments = new MusicArg
                {
                    Analyzer = _musicAnalysis,
                    Epoch = epoch,
                    SongData = _data,
                    IsContext = _analysisTypeContextSelected,
                    FilePath = selectedSongPath
                };
                // var objRef = _backgroundWorker.CreateObjRef(typeof(MusicAnalyzer));
                
                _backgroundWorker.RunWorkerAsync(arguments);

                // new Thread(new ThreadStart(Analyze))
                BtnRun.IsEnabled = false;
                LblProgress.Content = "Running...";
                
            }
        }

        private void Worker_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            MusicArg args = (MusicArg)e.Argument;

            Random r = new Random();

            if (args.Epoch <= 0)
            {
                double error = double.PositiveInfinity;
                double previous;
                do
                {
                    Shuffle(args.SongData, r);
                    previous = error;
                    error = 0;
                    for (int j = 0; j < args.SongData.Count; j++)
                    {
                        SongData song = args.SongData[j];

                        string songFile = System.IO.Path.Combine(args.FilePath, song.Source, song.Name + ".wav");
                        //// Console.WriteLine("Trying to load file...{0}", file);
                        AudioEngineCSharp.AudioEngine.Audio audio = AudioEngineCSharp.AudioEngine.Load(songFile, false, false);
                        //// load up the file


                        unsafe
                        {
                            // error += args.Analyzer.AddData(audio.RawData, audio.DataSize, args.IsContext ? song.Context : song.Theme, true);
                        }
                        //// _themeAnalyzer.AddData(a.Data, data.Theme);
                        audio.Dispose();

                        // float percent = ((float)(i * args.SongData.Count + (j + 1))) / totalNum;
                        // int progress = (int)(Math.Round(percent * 100));
                        Debug.WriteLine(string.Format("Finished ({0}) {1}", song.Source, song.Name));
                        // bw.ReportProgress(progress);
                    }
                    Debug.WriteLine(string.Format("Error: {0}, Previous: {1}", error, previous));
                // } while (Math.Abs(previous - error) >= 1e-10 * previous);
                } while (Math.Abs(previous - error) >= 1e-4);

            }
            else
            {

                int totalNum = args.SongData.Count * args.Epoch;

                // prepreprocess
                for (int i = 0; i < args.SongData.Count; i++)
                {
                    SongData song = args.SongData[i];
                    string songFile = System.IO.Path.Combine(args.FilePath, song.Source, song.Name + ".wav");
                    AudioEngineCSharp.AudioEngine.Audio audio = AudioEngineCSharp.AudioEngine.Load(songFile, false, false);

                    unsafe
                    {
                        args.Analyzer.PreprocessAddData(audio.RawData, audio.DataSize);
                    }
                    audio.Dispose();
                }

                args.Analyzer.PreDeviation();

                for (int i = 0; i < args.SongData.Count; i++)
                {
                    SongData song = args.SongData[i];
                    string songFile = System.IO.Path.Combine(args.FilePath, song.Source, song.Name + ".wav");
                    AudioEngineCSharp.AudioEngine.Audio audio = AudioEngineCSharp.AudioEngine.Load(songFile, false, false);

                    unsafe
                    {
                        args.Analyzer.ProcessDeviation(audio.RawData, audio.DataSize);
                    }
                    audio.Dispose();
                }

                args.Analyzer.PreProcessData();

                for (int i = 0; i < args.Epoch; i++)
                {
                    Shuffle(args.SongData, r);

                    for (int j = 0; j < args.SongData.Count; j++)
                    {
                        SongData song = args.SongData[j];

                        string songFile = System.IO.Path.Combine(args.FilePath, song.Source, song.Name + ".wav");
                        //// Console.WriteLine("Trying to load file...{0}", file);
                        AudioEngineCSharp.AudioEngine.Audio audio = AudioEngineCSharp.AudioEngine.Load(songFile, false, false);
                        //// load up the file


                        unsafe
                        {
                            args.Analyzer.ProcessData(audio.RawData, audio.DataSize, args.IsContext ? song.Context : song.Theme);
                        }
                        //// _themeAnalyzer.AddData(a.Data, data.Theme);
                        audio.Dispose();

                        float percent = ((float)(i * args.SongData.Count + (j + 1))) / totalNum;
                        int progress = (int)(Math.Round(percent * 100));
                        Debug.WriteLine(string.Format("Percent: {0}, Finished ({1}) {2}", percent, song.Source, song.Name));
                        bw.ReportProgress(progress);
                    }

                }
            }

        }

        private static void Shuffle<T>(List<T> list, Random r)
        {
            for (int j = 0; j < list.Count; j++)
            {
                int randVal = r.Next(j, list.Count);
                var temp = list[j];
                list[j] = list[randVal];
                list[randVal] = temp;
            }
        }

        private void Worker_UpdateProgress(object sender, ProgressChangedEventArgs e)
        {
            
            LblProgress.Content = e.ProgressPercentage + "%";
        }

        private void Worker_RunComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Save Results...",
                RestoreDirectory = true,
                Filter = "MusicAnalysis Files (*.musana)|*.musana|All files (*.*)|*.*"
            };

            bool? result = saveFileDialog.ShowDialog(this);
            if (result == true)
            {
                string file = saveFileDialog.FileName;
                _musicAnalysis.Save(file);
            }
            BtnRun.IsEnabled = true;
            LblProgress.Content = "Not Running";
            _musicAnalysis = null;
        }

        private void RdbContext_Checked(object sender, RoutedEventArgs e)
        {
            _analysisTypeContextSelected = true;
        }

        private void RdbTheme_OnChecked(object sender, RoutedEventArgs e)
        {
            _analysisTypeContextSelected = false;
        }

        private void BtnCompare_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "MusicAnalysis Files (*.musana)|*.musana|All files (*.*)|*.*",
                Multiselect = false,
                Title = "Open Music Analysis",
                RestoreDirectory = true
            };

            bool? result = openFile.ShowDialog();

            if (result == true)
            {
                MusicAnalyzer analysis = new MusicAnalyzer(openFile.FileName);
                Compare compare = new Compare(analysis);
                compare.ShowDialog();
            }
        }
    }



}
