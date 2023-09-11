using verse_interpreter.lib.Data;

namespace verse_interpreter.lib.Lookup.EventArguments
{
	public class UnBoundVariableDetectedEventArgs
	{
		public UnBoundVariableDetectedEventArgs(Variable variable)
		{
			Variable = variable;
		}

		public Variable Variable { get; }
	}
}