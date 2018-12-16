using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSVReadWriter
{
    // A really basic implementation. Does not handle quoted strings.
    public class CSVReader
    {
        private bool _dispose;
        private Stream _stream;

        private List<List<string>> _rows;

        public CSVReader(string fileLocation)
        {
            StreamReader streamReader;
            using (Stream stream = File.Open(fileLocation, FileMode.Open, FileAccess.Read))
            {
                streamReader = new StreamReader(stream);
            }
            
            

            _rows = new List<List<string>>();

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                // double quotes problem
                
                // line.Split(',');
                string[] columns = SplitLine(line);
            }

            
        }

        private string[] SplitLine(string line)
        {
            // remove quoted pairs
            
            // const string regexFindQuote = "(\".+?\")";

            // string noEsc = Regex.Replace(line, regexFindQuote, "");
            // int len = noEsc.Split(',').Length;
            // Regex.Split(line, )
            //string[] output = new string[len];
            //string curRegex = "";
            //for (int i = 0; i < len; i++)
            //{

            //    // curRegex = string.Join(",", curRegex, regexFindQuote);
            //}
            

            return line.Split(',');

        }
        
    }
}
