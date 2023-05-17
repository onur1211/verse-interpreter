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

    /// <summary>
    /// Wrapper class that combines the different evaluators in one class
    /// </summary>
    public class EvaluatorWrapper
    {
        public EvaluatorWrapper(IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>> arithmeticEvaluator,
                                IEvaluator<string, List<List<ExpressionResult>>> stringEvaluator)
        {
            ArithmeticEvaluator = arithmeticEvaluator;
            StringEvaluator = stringEvaluator;
        }

        public IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>> ArithmeticEvaluator { get; }
        public IEvaluator<string, List<List<ExpressionResult>>> StringEvaluator { get; }
    }
}
