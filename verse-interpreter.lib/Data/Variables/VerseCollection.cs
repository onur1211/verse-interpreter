using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.Variables;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib.Data
{
    public class VerseCollection
    {
        public VerseCollection(List<Variable> values)
        {
            this.Values = values;
            TypeData = new TypeData("any");
        }

        public TypeData TypeData { get; set; }

        public List<Variable> Values { get; set; }

        public bool HasValue()
        {
            return this.Values.Count > 0;
        }
    }
}
