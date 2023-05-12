using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Data
{
    public class DeclarationResult
    {
        public Nullable<int> Value { get; set; }

        public string Name { get; set; } = null!;

        public string TypeName { get; set; } = null!;
    }
}
