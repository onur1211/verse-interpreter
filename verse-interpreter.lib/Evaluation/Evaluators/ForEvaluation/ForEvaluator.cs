using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;

namespace verse_interpreter.lib.Evaluation.Evaluators.ForEvaluation
{
	public class ForEvaluator : IEvaluator<ForExpression, ForResult>
	{
		private PropertyResolver _propertyResolver;
		private readonly ApplicationState _applicationState;

		public ForEvaluator(PropertyResolver resolver,
							ApplicationState applicationState)
		{
			_propertyResolver = resolver;
			_applicationState = applicationState;
		}

		public bool AreVariablesBoundToValue(ForResult input)
		{
			throw new NotImplementedException();
		}

		public ForExpression Evaluate(ForResult input)
		{
			List<Variable> resultSequence = new List<Variable>();
			_applicationState.AddScope();

			PrepareLocalVariables(input.LocalVariables);
			ApplyFilter(input);
			resultSequence.AddRange(HandleArrayChoices(input));

			_applicationState.DropScope();

			return new ForExpression()
			{
				Collection = new VerseCollection(resultSequence)
			};
		}

		private void PrepareLocalVariables(List<Variable> localVariables)
		{
			foreach (var element in _applicationState.Scopes[_applicationState.CurrentScopeLevel - 1].LookupManager.GetAllVariables())
			{
				_applicationState.CurrentScope.AddScopedVariable(element);
			}
			foreach (var variable in localVariables)
			{
				_applicationState.CurrentScope.AddScopedVariable(variable);
			}
		}

		private void ApplyFilter(ForResult input)
		{
			// Not implemented yet
		}

		private List<Variable> HandleArrayChoices(ForResult input)
		{
			List<Variable> sequence = new List<Variable>();
			foreach (var choice in input.Choices)
			{
				foreach (var indexing in choice.IndexingResults)
				{
					sequence.AddRange(ExpandArrayToChoice(indexing.ArrayIdentifier, indexing.Indexer));
				}
			}

			return sequence;
		}

		private List<Variable> ExpandArrayToChoice(string arrayIdentifier, string indexingIdentifier)
		{
			var array = _propertyResolver.ResolveProperty(arrayIdentifier);
			var indexerVariable = _propertyResolver.ResolveProperty(indexingIdentifier);
			if (indexerVariable.HasValue())
			{
				throw new NotImplementedException();
			}

			indexerVariable.Value.IntValue = 0;

			List<Variable> result = new List<Variable>();
			while (indexerVariable.Value.IntValue != array.Value.CollectionVariable.Values.Count)
			{
				result.Add(array.Value.CollectionVariable.Values[indexerVariable.Value.IntValue.Value]);
				indexerVariable.Value.IntValue++;
			}

			return result;
		}

		private List<Variable> HandleExpressionChoice()
		{
			return null;
		}
		// Check if variables have value or not 
		// 
	}
}
