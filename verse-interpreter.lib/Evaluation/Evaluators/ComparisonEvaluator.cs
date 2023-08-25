using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Extensions;
using verse_interpreter.lib.Factories;

namespace verse_interpreter.lib.Evaluation.Evaluators
{
	public class ComparisonEvaluator : IEvaluator<ComparisonExpression, List<List<ExpressionResult>>>
	{
		private readonly PropertyResolver _resolver;
		private readonly ApplicationState _state;

		public ComparisonEvaluator(ApplicationState state, PropertyResolver resolver)
		{
			_resolver = resolver;
			_state = state;
		}

		public ComparisonExpression Evaluate(List<List<ExpressionResult>> input)
		{
			input.RemoveAll(x => x.Count == 0);
			ComparisonExpression lastExpression = new ComparisonExpression();
			if (!AreVariablesBoundToValue(input))
			{
				// If there are still unbound values in the expression, store it for later until the values are given
				lastExpression.Arguments = input.DeepClone();
				lastExpression.PostponedExpression = new Func<ComparisonExpression>(() => Evaluate(lastExpression.Arguments));

				return lastExpression;
			}

			input = SubstituteValues(input);
			foreach (var subExpression in input)
			{
				for (int i = 0; i < subExpression.Count; i++)
				{
					if (!string.IsNullOrEmpty(subExpression[i].Operator))
					{
						switch (subExpression[i].Operator)
						{
							case ">":
								EvaluateGreaterThan(lastExpression, subExpression[i - 1], subExpression[i + 1]);
								break;
							case "<":
								EvaluateSmallerThan(lastExpression, subExpression[i - 1], subExpression[i + 1]);
								break;
							case "=":
								EvaluateEqual(lastExpression, subExpression[i - 1], subExpression[i + 1]);
								break;
							case "<=":
								EvaluateSmallerThanOrEqual(lastExpression, subExpression[i - 1], subExpression[i + 1]);
								break;
							case ">=":
								EvaluateGreaterThanOrEqual(lastExpression, subExpression[i - 1], subExpression[i + 1]);
								break;
						}
					}
				}
			}

			return lastExpression!;
		}

		public bool AreVariablesBoundToValue(List<List<ExpressionResult>> input)
		{
			foreach (var subExpression in input)
			{
				foreach (var expression in subExpression)
				{
					if (string.IsNullOrEmpty(expression.ValueIdentifier))
					{
						continue;
					}

					var result = _resolver.ResolveProperty(expression.ValueIdentifier);
					if (!result.HasValue())
					{
						return false;
					}
				}
			}

			return true;
		}

		private List<List<ExpressionResult>> SubstituteValues(List<List<ExpressionResult>> input)
		{
			foreach (var subExpression in input)
			{
				foreach (var expression in subExpression)
				{
					if (!string.IsNullOrEmpty(expression.ValueIdentifier))
					{
						var variable = _resolver.ResolveProperty(expression.ValueIdentifier);
						switch (variable.Value.TypeData.Name)
						{
							case "string":
								expression.StringValue = variable.Value.StringValue;
								expression.ValueIdentifier = string.Empty;
								expression.TypeName = variable.Value.TypeData.Name;
								break;
							case "int":
								expression.IntegerValue = variable.Value.IntValue;
								expression.ValueIdentifier = string.Empty;
								expression.TypeName = variable.Value.TypeData.Name;
								break;
						}
					}
				}
			}

			return input;
		}

		private void EvaluateSmallerThan(ComparisonExpression expression, ExpressionResult firstOperand, ExpressionResult secondOperand)
		{
			if (firstOperand.IntegerValue != null)
			{
				expression.IntValue = firstOperand.IntegerValue < secondOperand.IntegerValue ? firstOperand.IntegerValue : null;
				return;
			}
			if (firstOperand.StringValue != null)
			{
				expression.StringValue = GetStringValue(firstOperand.StringValue) <  GetStringValue(secondOperand.StringValue) ? firstOperand.StringValue : null;
				return;
			}

			throw new NotImplementedException("Comparison for this type is not yet implemented!");
		}

		private void EvaluateGreaterThan(ComparisonExpression expression, ExpressionResult firstOperand, ExpressionResult secondOperand)
		{
			if (firstOperand.IntegerValue != null)
			{
				expression.IntValue = firstOperand.IntegerValue > secondOperand.IntegerValue ? firstOperand.IntegerValue : null;
				return;
			}
			if (firstOperand.StringValue != null)
			{
				expression.StringValue = GetStringValue(firstOperand.StringValue) > GetStringValue(secondOperand.StringValue) ? firstOperand.StringValue : null;
				return;
			}

			throw new NotImplementedException("Comparison for this type is not yet implemented!");
		}

		private void EvaluateEqual(ComparisonExpression expression, ExpressionResult firstOperand, ExpressionResult secondOperand)
		{
			if (firstOperand.IntegerValue != null)
			{
				expression.IntValue = firstOperand.IntegerValue == secondOperand.IntegerValue ? firstOperand.IntegerValue : null;
				return;
			}
			if (firstOperand.StringValue != null)
			{
				expression.StringValue = firstOperand.StringValue == secondOperand.StringValue ? firstOperand.StringValue : null;
				return;
			}

			throw new NotImplementedException("Comparison for this type is not yet implemented!");
		}

		private void EvaluateGreaterThanOrEqual(ComparisonExpression expression, ExpressionResult firstOperand, ExpressionResult secondOperand)
		{
			if (firstOperand.IntegerValue != null)
			{
				expression.IntValue = firstOperand.IntegerValue >= secondOperand.IntegerValue ? firstOperand.IntegerValue : null;
				return;
			}
			if (firstOperand.StringValue != null)
			{
				expression.StringValue = GetStringValue(firstOperand.StringValue) >= GetStringValue(secondOperand.StringValue) ? firstOperand.StringValue : null;
				return;
			}

			throw new NotImplementedException("Comparison for this type is not yet implemented!");
		}

		private void EvaluateSmallerThanOrEqual(ComparisonExpression expression, ExpressionResult firstOperand, ExpressionResult secondOperand)
		{
			if (firstOperand.IntegerValue != null)
			{
				expression.IntValue = firstOperand.IntegerValue <= secondOperand.IntegerValue ? firstOperand.IntegerValue : null;
				return;
			}
			if (firstOperand.StringValue != null)
			{
				expression.StringValue = GetStringValue(firstOperand.StringValue) <= GetStringValue(secondOperand.StringValue) ? firstOperand.StringValue : null;
				return;
			}

			throw new NotImplementedException("Comparison for this type is not yet implemented!");
		}


		/// <summary>
		/// Converts a string into the sum of the values of every single character similar to the Ord type class in haskell.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		private int GetStringValue(string input)
		{
			int counter = 0;
			foreach (char c in input)
			{
				counter += c;
			}

			return counter;
		}
	}
}
