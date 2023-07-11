using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.ResultObjects.Validators;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib.Evaluation.EvaluationManagement
{
    public class GeneralEvaluator
    {
        private readonly EvaluatorWrapper _evaluatorWrapper;
        private readonly BackpropagationEventSystem _propagator;
        private readonly ExpressionValidator _expressionValidator;

        public GeneralEvaluator(EvaluatorWrapper evaluatorWrapper,
                                BackpropagationEventSystem propagator,
                                ExpressionValidator expressionValidator)
        {
            _evaluatorWrapper = evaluatorWrapper;
            _propagator = propagator;
            _expressionValidator = expressionValidator;
        }

        public event EventHandler<ArithmeticExpressionResolvedEventArgs>? ArithmeticExpressionResolved;
        public event EventHandler<StringExpressionResolvedEventArgs>? StringExpressionResolved;
        public event EventHandler<ComparisonExpressionResolvedEventArgs>? ComparisonExpressionResolved;

        public BackpropagationEventSystem Propagator => _propagator;

        public void ExecuteExpression(List<List<ExpressionResult>> expressions, string? identifier = null)
        {
            if (!_expressionValidator.IsTypeConformityGiven(expressions))
            {
                throw new InvalidTypeCombinationException("The given expression contains multiple types!");
            }

            var typeName = _expressionValidator.GetExpressionType(expressions);

            switch (typeName)
            {
                case "string":
                    HandleStringExpression(expressions, identifier);
                    break;

                case "int":
                    HandleArithmeticExpression(expressions, identifier);
                    break;
                case "comparison":
                    HandleComparisonExpression(expressions, identifier);
                    break;

                default:
                    throw new UnknownTypeException(typeName);
            }
        }

        private void HandleComparisonExpression(List<List<ExpressionResult>> expressions, string? identifier)
        {
            var result = _evaluatorWrapper.ComparisonEvaluator.Evaluate(expressions);

            if (result.PostponedExpression != null)
            {
                _propagator.AddExpression(result);
                return;
            }

            ComparisonExpressionResolved?.Invoke(this, new ComparisonExpressionResolvedEventArgs(result));
        }

        private void HandleStringExpression(List<List<ExpressionResult>> expressions, string? identifier = null)
        {
            var result = _evaluatorWrapper.StringEvaluator.Evaluate(expressions);
            if(result.PostponedExpression != null && identifier != null)
            {
                _propagator.AddExpression(identifier, result);
                return;
            }
            if (result.PostponedExpression != null)
            {
                _propagator.AddExpression(result);
                return;
            }

            StringExpressionResolved?.Invoke(this, new StringExpressionResolvedEventArgs(result));
        }

        private void HandleArithmeticExpression(List<List<ExpressionResult>> expressions, string? identifier = null)
        {
            var result = _evaluatorWrapper.ArithmeticEvaluator.Evaluate(expressions);
            if (result.PostponedExpression != null && identifier != null)
            {
                _propagator.AddExpression(identifier, result);
                return;
            }
            if (result.PostponedExpression != null)
            {
                _propagator.AddExpression(result);
                return;
            }

            ArithmeticExpressionResolved?.Invoke(this, new ArithmeticExpressionResolvedEventArgs(result));
        }
    }
}
