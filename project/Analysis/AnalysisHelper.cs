using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Accord.IO;
using Accord.Math;
using MusicAnalysis;

namespace Analysis
{
    public class AnalysisHelper : IEnumerable
    {
        public int Count => _data.Count;

        private DataTable _worksheet;
        private List<SongData> _data;
        private MusicAnalyzer _contextAnalyzer;
        // private MusicAnalyzer _themeAnalyzer;
        private string _fileLocation;

        private class SongDataEnum : IEnumerator
        {
            private List<SongData> _data;
            private int index = -1;

            public SongDataEnum(List<SongData> l)
            {
                _data = l;
            }

            public bool MoveNext()
            {
                index++;
                return index < _data.Count;
            }

            public void Reset()
            {
                index = -1;
            }

            public object Current
            {
                get {
                    try
                    {
                        return _data[index];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        public AnalysisHelper(string fileLocation, string worksheetFile, string worksheetName)
        {
            _fileLocation = fileLocation;
            string worksheetLocation = Path.Combine(fileLocation, worksheetFile);
            Console.WriteLine("Path: {0}", worksheetLocation);
            ExcelReader reader = new ExcelReader(worksheetLocation);
            _worksheet = reader.GetWorksheet(worksheetName);
            _data = new List<SongData>();

            // string[] files = GetFromColumn<string>(_worksheet, "File");
            // Console.WriteLine(string.Join(", ", files));

            DataRowCollection rows = _worksheet.Rows;

            for (int i = 0; i < rows.Count; i++)
            {
                object[] a = _worksheet.Rows[i].ItemArray;
                
                string name = a[0].ToString();
                string source = a[1].ToString();
                string context = a[2].ToString();
                string theme = a[3].ToString();

                if (!(string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(source) &&
                    string.IsNullOrWhiteSpace(context)))
                {
                    _data.Add(new SongData()
                    {
                        Name = name,
                        Source = source,
                        Context = context,
                        Theme = theme
                    });
                }
            }

            List<string> contexts = _data.Select(a => a.Context).Distinct().ToList();
            _contextAnalyzer = new MusicAnalyzer(contexts);

            // List<string> themes = _data.Select(a => a.Theme).Distinct().ToList();
            // _themeAnalyzer = new MusicAnalyzer(themes);
        }

        public void PassFiles(int iterations = 1, string extension = ".wav")
        {
            for (int i = 0; i < iterations; i++)
            {
                foreach (SongData data in _data)
                {
                    string file = Path.Combine(_fileLocation, data.Source, data.Name + extension);
                    // Console.WriteLine("Trying to load file...{0}", file);
                    AudioEngineCSharp.AudioEngine.Audio a = AudioEngineCSharp.AudioEngine.Load(file, false, false);
                    // load up the file

                    // _contextAnalyzer.AddData(a.Data, data.Context);
                    // _themeAnalyzer.AddData(a.Data, data.Theme);
                    a.Dispose();
                }
            }

            // _contextAnalyzer.TrainData();
            // _themeAnalyzer.TrainData();
        }

        public Dictionary<string, double>[] TestFile(string fileLocation, string fileName, string extension = ".wav")
        {
            string file = Path.Combine(fileLocation, fileName + extension);
            AudioEngineCSharp.AudioEngine.Audio a = AudioEngineCSharp.AudioEngine.Load(file, false, false);
            var context = _contextAnalyzer.TestData(a.Data);
            // var theme = _themeAnalyzer.TestData(a.Data);
            a.Dispose();
            return new[] {context};
        }

        private static T[] GetFromColumn<T>(DataTable table, string columnName)
        {
            T[][] jagged = table.ToJagged<T>(columnName);
            return jagged?[0];
        }

        public void SaveAnalysis(string location)
        {

            using (FileStream s = File.Create(location))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                _contextAnalyzer.Save(formatter, s, SerializerCompression.GZip);
            }

        }

        public void LoadAnalysis(string location)
        {
            using (FileStream s = File.Open(location, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                _contextAnalyzer = formatter.Deserialize(s) as MusicAnalyzer;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new SongDataEnum(_data);
        }
    }
}
