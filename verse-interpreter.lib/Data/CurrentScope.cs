using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib.Data
{
    public class CurrentScope : IScope<int>
    {
        public Dictionary<int, IScope<int>> SubScope { get; private set; }

        public ILookupTable<int?> LookupTable { get; private set; }

        public LookupManager LookupManager { get; private set; }

        public CurrentScope()
        {
            LookupTable = new LookupTable<int?>();
            LookupManager = new LookupManager(LookupTable);
            SubScope = new Dictionary<int, IScope<int>>();
        }

        public void AddScopedVariable(int scopeId, DeclarationResult variable)
        {
            this.LookupManager.Add(variable);
        }
    }
}
