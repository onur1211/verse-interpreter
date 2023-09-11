namespace verse_interpreter.lib.Evaluators
{
	public interface IEvaluator<TOutput, WInput>
	{
		TOutput Evaluate(WInput input);

		bool AreVariablesBoundToValue(WInput input);
	}
}
