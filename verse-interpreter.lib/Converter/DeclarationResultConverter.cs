using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Converter
{
	public static class DeclarationResultConverter
	{
		public static DeclarationResult ConvertFunctionResult(FunctionCallResult result)
		{
			DeclarationResult declarationResult = new DeclarationResult();

			if (!result.WasValueResolved)
			{
				declarationResult.TypeName = "false?";
			}

			if (result.ArithmeticExpression != null)
			{
				declarationResult.LiteralValue = result.ArithmeticExpression.ResultValue.ToString()!;
				declarationResult.TypeName = "int";
			}

			if (result.StringExpression != null)
			{
				declarationResult.LiteralValue = result.StringExpression.Value;
				declarationResult.TypeName = "string";
			}

			if (result.ForExpression != null)
			{
				declarationResult.CollectionVariable = result.ForExpression.Collection;
				declarationResult.TypeName = "collection";
			}
			if (result.Variable != null)
			{
				declarationResult = VariableConverter.ConvertBack(result.Variable);
			}

			return declarationResult;
		}
	}
}
