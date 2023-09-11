using verse_interpreter.lib.Evaluation.FunctionEvaluator;
using verse_interpreter.lib.ParseVisitors.Functions;

namespace verse_interpreter.lib.Wrapper
{
	public class FunctionWrapper
	{
		public FunctionWrapper(FunctionDeclarationVisitor functionDeclarationVisitor, FunctionCallVisitor functionCallVisitor, FunctionCallPreprocessor functionCallPreprocessor)
		{
			FunctionDeclarationVisitor = functionDeclarationVisitor;
			FunctionCallVisitor = functionCallVisitor;
			FunctionCallPreprocessor = functionCallPreprocessor;
		}

		public FunctionDeclarationVisitor FunctionDeclarationVisitor { get; private set; }

		public FunctionCallVisitor FunctionCallVisitor { get; private set; }

		public FunctionCallPreprocessor FunctionCallPreprocessor { get; private set; }
	}
}
