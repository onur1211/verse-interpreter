using verse_interpreter.lib.Data;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib
{
    public class CustomType
    {
        private LookupManager _lookupManager;

        public string Name { get; set; }

        public string ConstructorName { get; set; }

        public CustomType()
        {
            Name = "undefinied";
            ConstructorName = "undefined";
            _lookupManager = new LookupManager();
        }

        private CustomType(LookupManager lookupManager, string name, string constructorName)
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

        public CustomType GetInstance()
        {
            return new CustomType(_lookupManager, Name, ConstructorName);
        }
    }
}
