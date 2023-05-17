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
        private int _level;

        public Dictionary<int, IScope<int>> SubScope { get; private set; }

        public LookupManager LookupManager { get; private set; }

        public ILookupTable<string> StringLookupTable {get; private set;}

        public ILookupTable<DynamicType> InstancesLookupTable { get; private set; }

        public ILookupTable<int?> IntLookupTable { get; private set; }

        public int Level { get { return _level; } set { _level = value; } }

        public CurrentScope(int level)
        {
            IntLookupTable = new LookupTable<int?>();
            StringLookupTable = new LookupTable<string>();
            InstancesLookupTable = new LookupTable<DynamicType>();
            LookupManager = new LookupManager(IntLookupTable, StringLookupTable, InstancesLookupTable);
            SubScope = new Dictionary<int, IScope<int>>();
            _level = level;
        }

        public void AddScopedVariable(int scopeId, DeclarationResult variable)
        {
            this.LookupManager.Add(variable);
        }
    }
}
