using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.IO
{
    // EXAM_UPDATED
    public class FileReader
    {
        public string ReadFileToEnd(string path)
        {
            using (StreamReader fs = new StreamReader(path))
            {
                return fs.ReadToEnd();
            }
        }
    }
}
