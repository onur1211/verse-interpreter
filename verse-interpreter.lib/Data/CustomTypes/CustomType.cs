using verse_interpreter.lib.Data;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib.Data.CustomTypes
{
    public struct CustomType
    {
        private LookupManager _lookupManager;

        public string Name { get; set; }

        public string ConstructorName { get; set; }

        public CustomType()
        {
            Name = "undefined";
            ConstructorName = "undefined";
            _lookupManager = new LookupManager();
        }

        public LookupManager LookupManager { get { return _lookupManager; } set { _lookupManager = value; } }

        public void AddScopedVariable(Variable variable)
        {
            _lookupManager.AddVariable(variable);
        }
    }
}
