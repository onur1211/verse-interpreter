using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Extensions;

namespace verse_interpreter.lib.Evaluators
{
    public class StringExpressionEvaluator : IEvaluator<StringExpression, List<List<ExpressionResult>>>
    {
        private ApplicationState _applicationState;
        private readonly PropertyResolver _resolver;

        public StringExpressionEvaluator(ApplicationState applicationState, PropertyResolver resolver)
        {
            _applicationState = applicationState;
            _resolver = resolver;
        }

        public StringExpression Evaluate(List<List<ExpressionResult>> input)
        {
            StringBuilder concatinatedString = new StringBuilder();
            StringExpression expression = new StringExpression();
            if (!AreVariablesBoundToValue(input))
            {
                expression.Arguments = input.DeepClone();
                expression.PostponedExpression = new Func<StringExpression>(() =>
                {
                    return Evaluate(expression.Arguments);
                });

                return expression;
            }

            foreach (var subString in input)
            {
                for (int i = 0; i < subString.Count; i++)
                {
                    // Binary operation using the first and third element as operands
                    if (!string.IsNullOrEmpty(subString[i].Operator) && subString[i].Operator == "+" &&
                        concatinatedString.Length == 0)
                    {
                        concatinatedString.Append(Add(subString[i - 1], subString[i + 1]));
                        continue;
                    }
                    // In all other cases simply add the next element to the existing chain
                    if (!string.IsNullOrEmpty(subString[i].Operator) && subString[i].Operator == "+")
                    {
                        var addedString = Add(concatinatedString.ToString(), subString[i + 1]);
                        concatinatedString = new StringBuilder(addedString);
                    }
                }
            }
            expression.Value = concatinatedString.ToString();
            return expression;
        }

        private string GetValue(ExpressionResult expressionResult)
        {
            if (!string.IsNullOrEmpty(expressionResult.ValueIdentifier))
            {
                return _resolver.ResolveProperty(expressionResult.ValueIdentifier).Value.StringValue;
            }
            if(expressionResult.StringValue != null)
            {
                return expressionResult.StringValue.Replace("\"", "");
            }

            throw new NotImplementedException("The given expression contains no, or unknown data!");
        }

        private string Add(ExpressionResult firstExpressionResult, ExpressionResult secondExpressionResult)
        {
            var firstValue = GetValue(firstExpressionResult);
            var secondValue = GetValue(secondExpressionResult);

            return firstValue + secondValue;
        }

        private string Add(string expression, ExpressionResult expressionResult)
        {
            return expression + GetValue(expressionResult);
        }

        public bool AreVariablesBoundToValue(List<List<ExpressionResult>> input)
        {
            foreach (var expressionResult in input)
            {
                foreach (var subExpression in expressionResult)
                {
                    if (!string.IsNullOrEmpty(subExpression.ValueIdentifier) && subExpression.ValueIdentifier.Contains('.'))
                    {
                        var identfieres = subExpression.ValueIdentifier.Split('.');
                        var instanceVariable = _applicationState.CurrentScope.LookupManager.GetVariable(identfieres[0]).Value.CustomType;
                        string result = _applicationState.CurrentScope.LookupManager.GetMemberVariable(instanceVariable, identfieres[0], identfieres[1]).Value.StringValue;
                        if (result == null)
                        {
                            return false;
                        }
                        continue;
                    }

                    if (!string.IsNullOrEmpty(subExpression.ValueIdentifier) && !_applicationState.CurrentScope.LookupManager.HasValue(subExpression.ValueIdentifier))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
