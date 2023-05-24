using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib.Data
{
    public class CurrentScope : IScope<Variable>
    {
        private int _level;

        public Dictionary<int, IScope<Variable>> SubScope { get; private set; }

        public LookupManager LookupManager { get; private set; }

        public int Level { get { return _level; } set { _level = value; } }

        public FunctionLookupManager FunctionLookupManager { get; private set; }

        public CurrentScope(int level)
        {
            LookupManager = new LookupManager();
            SubScope = new Dictionary<int, IScope<Variable>>();
            _level = level;
        }

        public void AddScopedVariable(Variable variable)
        {
            this.LookupManager.AddVariable(variable);
        }

        public void AddFunction(Function function)
        {
            this.LookupManager.AddFunction(function);
        }
    }
}
