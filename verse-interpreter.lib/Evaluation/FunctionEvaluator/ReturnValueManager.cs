using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Evaluation.FunctionEvaluator
{
	public class ReturnValueManager
	{
		private ArithmeticExpression? ArithmeticExpression { get; set; }
		private StringExpression? StringExpression { get; set; }
		private ForExpression? ForExpression { get; set; }
		private Variable? Variable { get; set; }
		public bool WasValueResolved { get; set; }

		public FunctionCallResult BuildResult()
		{
			return new FunctionCallResult()
			{
				ArithmeticExpression = ArithmeticExpression,
				StringExpression = StringExpression,
				ForExpression = ForExpression,
				Variable = Variable,
				WasValueResolved = WasValueResolved,
			};
		}

		public void OnResultEvaluated(ArithmeticExpression arithmeticExpression)
		{
			ArithmeticExpression = arithmeticExpression;
		}

		public void OnResultEvaluated(StringExpression stringExpression)
		{
			StringExpression = stringExpression;
		}

		public void OnResultEvaluated(ForExpression forExpression)
		{
			ForExpression = forExpression;
		}

		public void OnVariableResolved(Variable variable)
		{
			Variable = variable;
		}

		public void OnResultEvaluated(ExpressionWithNoValueFoundEventArgs eventArgs)
		{
			WasValueResolved = false;
		}
	}
}
