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

        public LookupManager LookupManager { get; private set; }

        public ILookupTable<string> StringLookupTable {get; private set;}

        public ILookupTable<int?> IntLookupTable { get; private set; }

        public CurrentScope()
        {
            IntLookupTable = new LookupTable<int?>();
            StringLookupTable = new LookupTable<string>();
            LookupManager = new LookupManager(IntLookupTable, StringLookupTable);
            SubScope = new Dictionary<int, IScope<int>>();
        }

        public void AddScopedVariable(int scopeId, DeclarationResult variable)
        {
            this.LookupManager.Add(variable);
        }
    }
}
