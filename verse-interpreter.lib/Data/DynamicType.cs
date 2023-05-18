using verse_interpreter.lib.Data;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib
{
    public class DynamicType
    {
        private LookupManager _lookupManager;

        public string Name { get; set; }

        public string ConstructorName { get; set; }

        public DynamicType()
        {
            Name = "undefinied";
            ConstructorName = "undefined";
            _lookupManager = new LookupManager();
        }

        private DynamicType(LookupManager lookupManager, string name, string constructorName)
        {
            _lookupManager = lookupManager;
            Name = name;
            ConstructorName = constructorName;
        }

        public LookupManager LookupManager { get { return _lookupManager; } }

        public void AddScopedVariable(Variable variable)
        {
            _lookupManager.AddVariable(variable);
        }

        public DynamicType GetInstance()
        {
            return new DynamicType(_lookupManager, Name, ConstructorName);
        }
    }
}
