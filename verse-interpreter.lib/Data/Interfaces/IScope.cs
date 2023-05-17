using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib.Data
{
    public interface IScope<T>
    {
        Dictionary<int, IScope<T>> SubScope { get; }

        LookupManager LookupManager { get; }

        int Level { get; set; }

        void AddScopedVariable(int scopeId, Variable variable);
    }
}
