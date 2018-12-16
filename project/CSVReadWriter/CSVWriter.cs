using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVReadWriter
{
    public class CSVWriter
    {
        private bool _dispose;
        private Stream _stream;

        public CSVWriter(string fileLocation) : this(File.Open(fileLocation, FileMode.Open, FileAccess.ReadWrite))
        {
        }

        public CSVWriter(Stream s, bool dispose = true)
        {
            _stream = s;
            _dispose = dispose;
        }

        ~CSVWriter()
        {
            if (_dispose)
                _stream.Dispose();
            

        }




    }
}
