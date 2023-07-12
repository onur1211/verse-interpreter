using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Converter
{
	public static class DeclarationResultConverter
	{
		public static DeclarationResult ConvertFunctionResult(FunctionCallResult result)
		{
			DeclarationResult declarationResult = new DeclarationResult();
			if(result.ArithmeticExpression != null)
			{
				declarationResult.Value = result.ArithmeticExpression.ResultValue.ToString()!;
				declarationResult.TypeName = "int";
			}
			if (result.StringExpression != null)
			{
				declarationResult.Value = result.StringExpression.Value;
				declarationResult.TypeName = "string";
			}

			return declarationResult;
		}
	}
}
