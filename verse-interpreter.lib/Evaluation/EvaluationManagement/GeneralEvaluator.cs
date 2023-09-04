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
        private readonly PropertyResolver _propertyResolver;
        private readonly BackpropagationEventSystem _propagator;
        private readonly ExpressionValidator _expressionValidator;

        public GeneralEvaluator(EvaluatorWrapper evaluatorWrapper,
                                PropertyResolver propertyResolver,
                                BackpropagationEventSystem propagator,
                                ExpressionValidator expressionValidator)
        {
            _evaluatorWrapper = evaluatorWrapper;
            _propertyResolver = propertyResolver;
            _propagator = propagator;
            _expressionValidator = expressionValidator;
        }

        public event EventHandler<ArithmeticExpressionResolvedEventArgs>? ArithmeticExpressionResolved;
        public event EventHandler<StringExpressionResolvedEventArgs>? StringExpressionResolved;
        public event EventHandler<ComparisonExpressionResolvedEventArgs>? ComparisonExpressionResolved;
        public event EventHandler<ExpressionWithNoValueFoundEventArgs>? ExpressionWithNoValueFound;
        public event EventHandler<ForExpressionResolvedEventArgs>? ForExpressionResolved;

        public BackpropagationEventSystem Propagator => _propagator;

        public void ExecuteExpression(List<List<ExpressionResult>> expressions, string? identifier = null)
        {
            if (!_expressionValidator.IsTypeConformityGiven(expressions))
            {
                throw new InvalidTypeCombinationException("The given expression contains multiple types!");
            }

            // Check if false? is contained anywhere in the expression
            // and invoke the event for the value definition visitor.
            if (expressions.Any(x => x.Any(y => y.TypeName == "false?")))
            {
                ExpressionWithNoValueFound?.Invoke(this, new ExpressionWithNoValueFoundEventArgs());
                return;
            }

            // Check if a collection (example: myArray[0]) 
            // and invoke the event for the value definition visitor.
            if (expressions.Any(x => x.Any(y => y.TypeName == "collection")))
            {
                var filteredList = expressions.Where(x => x.Any(y => y.TypeName == "collection"));

                foreach (var expression in filteredList)
                {
                    var arrayIndex = expression.Where(x => x.TypeName == "collection");

                    foreach (var value in arrayIndex)
                    {
                        if (_propertyResolver.ResolveProperty(value.ValueIdentifier).Value.TypeData.Name == "false?")
                        {
                            ExpressionWithNoValueFound?.Invoke(this, new ExpressionWithNoValueFoundEventArgs());
                            return;
                        }
                    }
                }
            }

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

                case "false?":
                    ExpressionWithNoValueFound?.Invoke(this, new ExpressionWithNoValueFoundEventArgs());
                    return;

                default:
                    throw new UnknownTypeException(typeName);
            }
        }

        public void ExecuteExpression(ForResult forExpression, string? identifier = null)
        {
            var res = _evaluatorWrapper.ForEvaluator.Evaluate(forExpression);
            if (res.PostponedExpression != null)
            {
                throw new NotImplementedException();
            }

            ForExpressionResolved?.Invoke(this, new ForExpressionResolvedEventArgs(res));
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
