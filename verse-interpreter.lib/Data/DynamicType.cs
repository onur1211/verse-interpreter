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
            _lookupManager = new LookupManager(new LookupTable<int?>(), new LookupTable<string>(), new LookupTable<DynamicType>());
        }

        private DynamicType(LookupManager lookupManager, string name, string constructorName)
        {
            _lookupManager = lookupManager;
            Name = name;
            ConstructorName = constructorName;
        }

        public LookupManager LookupManager { get { return _lookupManager; } }

        public void AddScopedVariable(DeclarationResult result)
        {
            _lookupManager.Add(result);
        }

        public DynamicType GetInstance()
        {
            return new DynamicType(_lookupManager, Name, ConstructorName);
        }
    }
}
