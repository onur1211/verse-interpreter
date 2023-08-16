using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.ResultObjects.Expressions;
using verse_interpreter.lib.Data.Variables;
using verse_interpreter.lib.Evaluators;

namespace verse_interpreter.lib.Evaluation.Evaluators.ForEvaluation
{
	public class ForEvaluator : IEvaluator<ForExpression, ForResult>
	{
		private PropertyResolver _propertyResolver;
		private readonly ApplicationState _applicationState;
		private readonly FilterApplyer _filterApplyer;
		private List<ExpressionSet> _filters;

		public ForEvaluator(PropertyResolver resolver,
							ApplicationState applicationState,
							FilterApplyer filterApplyer)
		{
			_propertyResolver = resolver;
			_applicationState = applicationState;
			_filterApplyer = filterApplyer;
			_filters = null!;
		}

		public bool AreVariablesBoundToValue(ForResult input)
		{
			throw new NotImplementedException();
		}

		public ForExpression Evaluate(ForResult input)
		{
			List<Variable> resultSequence = new List<Variable>();

			_filters = input.Filters;

			_applicationState.AddScope();
			PrepareLocalVariables(input.LocalVariables);
			resultSequence.AddRange(TraverseChoices(input));

			_applicationState.DropScope();
			_filters.Clear();
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

		private List<Variable> TraverseChoices(ForResult input)
		{
			List<Variable> sequence = new List<Variable>();
			var current = input.Choices;

			while (current != null)
			{
				foreach (var indexing in current.IndexingResults)
				{
					sequence.AddRange(ExpandArrayToChoice(indexing.ArrayIdentifier, indexing.Indexer));
				}
				foreach (var literal in current.Literals)
				{
					sequence.Add(literal);
				}

				current = current.Next;
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

			indexerVariable = ExpandVariable(indexerVariable, array);
			var indexerChoice = indexerVariable.Value.Choice;
			List<Variable> result = new List<Variable>();

			foreach (var choice in indexerChoice.AllChoices())
			{
				indexerVariable.Value.IntValue = choice.ValueObject.IntValue;
				var returnedValue = array.Value.CollectionVariable.Values[choice.ValueObject.IntValue!.Value];
				if (_filterApplyer.DoesFilterMatch(this._filters, indexerVariable))
				{
					result.Add(returnedValue);
				}
			}

			return result;
		}

		private Variable ExpandVariable(Variable counterVariable, Variable collectionVariable)
		{
			if (counterVariable.Value.Choice != null)
			{
				return counterVariable;
			}
			else
			{
				counterVariable.Value.Choice = new Choice(counterVariable.Value);
			}

			for (int i = 0; i < collectionVariable.Value.CollectionVariable.Values.Count; i++)
			{
				counterVariable.Value.Choice.AddValue(i);
			}


			return counterVariable;
		}
	}
}
