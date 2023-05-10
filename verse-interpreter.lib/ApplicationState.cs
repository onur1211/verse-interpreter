using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;

namespace verse_interpreter.lib
{
    public class ApplicationState
    {
        public Dictionary<string, IScope<int>> Scopes { get; set; }

        public ApplicationState()
        {
            Scopes = new Dictionary<string, IScope<int>>();
        }
    }
}
