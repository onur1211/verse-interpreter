using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Factories;

namespace verse_interpreter.lib
{
    public class ExpressionValidator
    {
        private IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>> _arithmeticEvaluator;
        private IEvaluator<string, List<List<ExpressionResult>>> _stringEvaluator;

        public ExpressionValidator(IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>> arithmeticEvaluator,
                                   IEvaluator<string, List<List<ExpressionResult>>> stringEvaluator)
        {
            _arithmeticEvaluator = arithmeticEvaluator;
            _stringEvaluator = stringEvaluator;
        }

        /// <summary>
        /// Checks whether or not all expression elements are of the same type.
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public bool IsTypeConformityGiven(List<List<ExpressionResult>> expressions)
        {
            foreach (var expression in expressions)
            {
                foreach(var exp in expression)
                {
                }
            }
            throw new NotImplementedException();
        }
    }
}
