using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.Validators;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib
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
        public event EventHandler<StringExpressionResolved>? StringExpressionResolved;


        public void ExecuteExpression(List<List<ExpressionResult>> expressions)
        {
            if (!_expressionValidator.IsTypeConformityGiven(expressions))
            {
                throw new InvalidTypeCombinationException("The given expression contains multiple types!");
            }
            var typeName = _expressionValidator.GetExpressionType(expressions);

            switch(typeName)
            {
                case "string":
                    HandleStringExpression(expressions);
                    break;

                case "int":
                    HandleArithmeticExpression(expressions);
                    break;

                default:
                    throw new UnknownTypeException(typeName);
            }
        }

        private void HandleStringExpression(List<List<ExpressionResult>> expressions)
        {
            var result = _evaluatorWrapper.StringEvaluator.Evaluate(expressions);
        }

        private void HandleArithmeticExpression(List<List<ExpressionResult>> expressions)
        {
            var result = _evaluatorWrapper.ArithmeticEvaluator.Evaluate(expressions);
        }
    }
}
