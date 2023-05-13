using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Lookup
{
    public class LookupTable<T>
    {
        public LookupTable() 
        {
            this.Table = new Dictionary<string, List<T>>();
        }

        public Dictionary<string, List<T>> Table { get; set; }
    }
}
