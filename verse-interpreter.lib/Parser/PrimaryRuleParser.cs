using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.ParseVisitors.Functions;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.Parser
{
    public class PrimaryRuleParser
	{
		private readonly ApplicationState _applicationState;
		private readonly Lazy<FunctionCallVisitor> _functionCallVisitor;

		public PrimaryRuleParser(ApplicationState applicationState, Lazy<FunctionCallVisitor> functionCallVisitor)
		{
			_applicationState = applicationState;
			_functionCallVisitor = functionCallVisitor;
		}

		public ExpressionResult ParsePrimary([Antlr4.Runtime.Misc.NotNull] Verse.PrimaryContext context)
		{
			ExpressionResult result = new ExpressionResult();
			// Checks if the there are any subexpressions --> due to brackets for instance
			// Fetches the value / identifer from the current node
			var fetchedValue = context.INT();
			var fetchedIdentifier = context.ID();
            var fetchedNoValue = context.NOVALUE();
            var fetchedMemberAccess = context.type_member_access();
			var fetchedString = context.string_rule();
			var fetchedArrayAccess = context.array_index();
			var fetchedFunctionCall = context.function_call();

			if (fetchedValue != null)
			{
				result.IntegerValue = Convert.ToInt32(fetchedValue.ToString());
				result.TypeName = "int";
				return result;
			}

            if (fetchedNoValue != null)
            {
                result.TypeName = "false?";
                return result;
            }

            if (fetchedIdentifier != null)
			{
				result.ValueIdentifier = fetchedIdentifier.GetText();
				return result;

			}

			if (fetchedMemberAccess != null)
			{
				result.ValueIdentifier = fetchedMemberAccess.GetText();
				return result;
			}

			if (fetchedString != null)
			{
				result.StringValue = fetchedString.GetText();
				result.TypeName = "string";
				return result;
			}

			if (fetchedArrayAccess != null)
			{
				result.ValueIdentifier = fetchedArrayAccess.GetText();
				result.TypeName = "collection";
				return result;
			}

			if (fetchedFunctionCall != null)
			{
				var returnedFunctionValue = fetchedFunctionCall.Accept(_functionCallVisitor.Value);
				result = HandleIntResult(returnedFunctionValue, result);
				result = HandleStringResult(returnedFunctionValue, result);
				return result;
			}

			throw new NotImplementedException();
		}

		private ExpressionResult HandleStringResult(FunctionCallResult result, ExpressionResult expressionResult)
		{
			if (result.StringExpression == null)
			{
				return expressionResult;
			}

			expressionResult.StringValue = result.StringExpression.Value;
			expressionResult.TypeName = "string";
			return expressionResult;
		}

		private ExpressionResult HandleIntResult(FunctionCallResult result, ExpressionResult expressionResult)
		{
			if (result.ArithmeticExpression == null)
			{
				return expressionResult;
			}

			expressionResult.IntegerValue = result.ArithmeticExpression.ResultValue;
			expressionResult.TypeName = "int";
			return expressionResult;
		}
	}
}