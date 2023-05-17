using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Lookup
{
    public class LookupTable<T> : ILookupTable<T>
    {
        public LookupTable() 
        {
            this.Table = new Dictionary<string, T>();
        }

        public Dictionary<string, T> Table { get; set; }
    }
}
