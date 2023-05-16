using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib
{
    // EXAM_UPDATED
    public class DynamicType
    {
        private LookupManager _lookupManager;

        public string Name { get; set; }
        public string ConstructorName { get; set; }
        public DynamicType()
        {
            Name = "undefinied";
            ConstructorName = "undefined";
            _lookupManager = new LookupManager(new LookupTable<int?>(), new LookupTable<string>());
        }

        public void AddScopedVariable(DeclarationResult result)
        {
            _lookupManager.Add(result);
        }
    }
}
