using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;

namespace verse_interpreter.lib.Evaluation.Evaluators.ForEvaluation
{
	public class ForEvaluator : IEvaluator<object, ForResult>
	{
		private PropertyResolver _propertyResolver;

		public ForEvaluator(PropertyResolver resolver)
        {
				_propertyResolver = resolver;
        }

        public bool AreVariablesBoundToValue(ForResult input)
		{
			throw new NotImplementedException();
		}

		public object Evaluate(ForResult input)
		{
			throw new NotImplementedException();
		}

		private void PrepareLocalVariables()
		{

		}

		private void HandleArrayChoices()
		{

		}
		// Check if variables have value or not 
		// 
	}
}
