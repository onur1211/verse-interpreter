using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Evaluation.EvaluationManagement
{
	public class IfExpressionResolvedEventArgs
	{
		public IfExpressionResolvedEventArgs(IfParseResult result, bool isSuccess)
		{
			IsSuccess = isSuccess;
			Result = result;
		}

		public bool IsSuccess { get; }
		public IfParseResult Result { get; }

	}
}