using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Lookup
{
    public interface ILookupTable<T>
    {
        public Dictionary<string, List<T>> Table { get; set; }
    }
}
