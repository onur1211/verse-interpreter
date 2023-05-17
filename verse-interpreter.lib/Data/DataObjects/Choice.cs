using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Data.DataObjects
{
    public class Choice<T>
    {
        public string Name { get; set; }

        public List<T> PossibleValues { get; set; }
    }
}
