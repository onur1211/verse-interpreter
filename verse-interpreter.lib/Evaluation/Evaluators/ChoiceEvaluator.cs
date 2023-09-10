using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Variables;

namespace verse_interpreter.lib.Evaluation.Evaluators
{
	public class ChoiceEvaluator
	{
		private readonly ApplicationState applicationState;

		public ChoiceEvaluator(ApplicationState applicationState)
		{
			this.applicationState = applicationState;
		}

		public List<TOutput> EvaluateChoice<TOutput>(Variable variable, Func<TOutput> func)
		{
			List<TOutput> result = new List<TOutput>();
			foreach (var item in variable.Value.Choice.AllChoices())
			{
				variable.Value = item.ValueObject;
				applicationState.CurrentScope.LookupManager.UpdateVariable(variable);
				result.Add(func());
			}

			return result;
		}
	}
}
