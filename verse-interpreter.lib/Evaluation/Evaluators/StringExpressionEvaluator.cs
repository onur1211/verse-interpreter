using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Evaluators
{
    public class StringExpressionEvaluator : IEvaluator<string, List<List<ExpressionResult>>>
    {
        private ApplicationState _applicationState;

        public StringExpressionEvaluator(ApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        public string Evaluate(List<List<ExpressionResult>> input)
        {
            StringBuilder concatinatedString = new StringBuilder();
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
            return concatinatedString.ToString();
        }

        private string GetValue(ExpressionResult expressionResult)
        {
            if (!string.IsNullOrEmpty(expressionResult.ValueIdentifier))
            {
                return _applicationState.CurrentScope.LookupManager.GetVariable(expressionResult.ValueIdentifier).Value.StringValue;
            }

            return null;
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
            throw new NotImplementedException();
        }
    }
}
