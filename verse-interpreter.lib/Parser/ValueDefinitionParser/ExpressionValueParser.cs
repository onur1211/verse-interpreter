using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;

namespace verse_interpreter.lib.Parser.ValueDefinitionParser
{
	public class ExpressionValueParser
	{
		private readonly GeneralEvaluator _evaluator;

		public ExpressionValueParser(GeneralEvaluator generalEvaluator)
		{
			_evaluator = generalEvaluator;
		}

		public DeclarationResult? ParseExpression(List<List<ExpressionResult>> expression)
		{
			DeclarationResult declarationResult = new DeclarationResult
			{
				ExpressionResults = expression
			};
			_evaluator.ArithmeticExpressionResolved += (x, y) =>
			{
				declarationResult.LiteralValue = y.Result.ResultValue.ToString()!;
				declarationResult.ExpressionResults = null;
				declarationResult.TypeName = "int";
			};
			_evaluator.StringExpressionResolved += (x, y) =>
			{
				declarationResult.ExpressionResults = null;
				declarationResult.LiteralValue = y.Result.Value;
				declarationResult.TypeName = "string";
			};
			_evaluator.ExpressionWithNoValueFound += (x, y) =>
			{
				declarationResult.TypeName = "false?";

			};
			_evaluator.ExecuteExpression(expression);

			return declarationResult;
		}

		public DeclarationResult ParseForExpression(ForResult result)
		{
			DeclarationResult declarationResult = new DeclarationResult();
			_evaluator.ForExpressionResolved += (x, y) =>
			{
				declarationResult.CollectionVariable = y.ForExpression.Collection!;
				declarationResult.TypeName = "collection";
			};
			_evaluator.ExecuteExpression(result);

			return declarationResult;
		}
	}
}
