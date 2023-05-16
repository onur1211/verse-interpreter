using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Factories;

namespace verse_interpreter.lib.Evaluators
{
    public class ArithmeticEvaluator : IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>>
    {
        private ApplicationState _state;

        public ArithmeticEvaluator(ApplicationState applicationState)
        {
            _state = applicationState;
        }

        public ArithmeticExpression Evaluate(List<List<ExpressionResult>> input)
        {
            // Remove any empty lists from the input
            input.RemoveAll(x => x.Count == 0);
            ArithmeticExpression lastExpression = new ArithmeticExpression();

            foreach (var expression in input)
            {
                // Substitute variable values with their corresponding integer values
                var exp = SubstituteValues(expression);

                // Check the number of expression items
                if (exp.Count == 3)
                {
                    // Build a binary expression
                    lastExpression = BuildBinaryExpression(lastExpression, exp);
                }
                if (exp.Count == 2)
                {
                    // Bind to an existing binary expression
                    lastExpression = BindToExistingBinaryExpression(lastExpression, expression);
                }
                if (exp.Count == 1)
                {
                    // Merge two expression blocks
                    lastExpression = MergeTwoExpressionBlocks(lastExpression, expression);
                }
                if(exp.Count > 3)
                {
                    lastExpression = BuildSimpleComposedExpression(lastExpression, expression);
                }
                //Console.WriteLine($"{lastExpression.StringRepresentation} | {input.IndexOf(expression)}");
            }

            // If the result value of the last expression is not set, evaluate the string representation
            if (!lastExpression.ResultValue.HasValue)
            {
                lastExpression.ResultValue = Z.Expressions.Eval.Execute<int>(lastExpression.StringRepresentation);
            }

            return lastExpression;
        }

        private List<ExpressionResult> SubstituteValues(List<ExpressionResult> input)
        {
            List<ExpressionResult> results = new List<ExpressionResult>();
            foreach (var expressionResult in input)
            {
                if (!string.IsNullOrEmpty(expressionResult.ValueIdentifier))
                {
                    // Lookup the variable value and substitute it in the expression
                    var result = _state.CurrentScope.LookupManager.GetVariableInts(expressionResult.ValueIdentifier).First();
                    expressionResult.IntegerValue = result.Value;
                    expressionResult.ValueIdentifier = string.Empty;
                    results.Add(expressionResult);
                }
                else
                {
                    results.Add(expressionResult);
                }
            }

            return results;
        }

        private ArithmeticExpression BuildBinaryExpression(ArithmeticExpression lastExpression, List<ExpressionResult> expressionResult)
        {
            if (lastExpression == null)
            {
                lastExpression = new ArithmeticExpression();
            }

            if (!string.IsNullOrEmpty(expressionResult[1].Operator))
            {
                // Evaluate the binary expression and update the string representation
                var evaluation = Z.Expressions.Eval.Execute(expressionResult[0].IntegerValue + expressionResult[1].Operator + expressionResult[2].IntegerValue);
                lastExpression.StringRepresentation += evaluation.ToString();
                return lastExpression;
            }

            throw new InvalidDataException();
        }

        private ArithmeticExpression BindToExistingBinaryExpression(ArithmeticExpression lastExpression, List<ExpressionResult> expressionResult)
        {
            if (lastExpression == null)
            {
                // Create a new arithmetic expression with the string representation of the two expression items
                return new ArithmeticExpression()
                {
                    StringRepresentation = expressionResult[0].IntegerValue + expressionResult[1].Operator
                };
            }

            if (!string.IsNullOrEmpty(expressionResult[0].Operator))
            {
                // Evaluate the existing arithmetic expression and create a new one by appending the second expression item
                var evaluation = Z.Expressions.Eval.Execute<int>(lastExpression.StringRepresentation);
                return new ArithmeticExpression()
                {
                    StringRepresentation = evaluation.ToString() + expressionResult[0].Operator + expressionResult[1].IntegerValue,
                };
            }
            else
            {
                // Append the second expression item to the existing string representation
                var stringRepresentation = lastExpression.StringRepresentation + expressionResult[0].IntegerValue + expressionResult[1].Operator;
                return new ArithmeticExpression()
                {
                    StringRepresentation = stringRepresentation
                };
            }
        }

        private ArithmeticExpression MergeTwoExpressionBlocks(ArithmeticExpression lastEpxression, List<ExpressionResult> expressionResults)
        {
            // Evaluate the existing arithmetic expression and create a new one by wrapping it in parentheses and appending the operator
            var evaluation = Z.Expressions.Eval.Execute(lastEpxression.StringRepresentation);
            return new ArithmeticExpression()
            {
                StringRepresentation = $"({evaluation}){expressionResults[0].Operator}"
            };
        }

        private ArithmeticExpression BuildSimpleComposedExpression(ArithmeticExpression lastEpxression, List<ExpressionResult> expressionResults)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var expressionResult in expressionResults)
            {
                if (!string.IsNullOrEmpty(expressionResult.Operator))
                {
                    builder.Append(expressionResult.Operator);
                }
                else
                {
                    builder.Append(expressionResult.IntegerValue);
                }
            }

            return new ArithmeticExpression()
            {
                StringRepresentation = builder.ToString()
            };
        }
    }
}

