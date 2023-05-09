using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Data
{
    public abstract class Variable<T>
    {
        public string Name { get; set; } = null!;

        public T Value { get; set; } = default!;
    }
}
