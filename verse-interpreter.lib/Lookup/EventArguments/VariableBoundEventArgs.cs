using verse_interpreter.lib.Data;

namespace verse_interpreter.lib.Lookup.EventArguments
{
    public class VariableBoundEventArgs
    {
        public VariableBoundEventArgs(Variable variable)
        {
            Variable = variable;
        }

        public Variable Variable { get; }
    }
}