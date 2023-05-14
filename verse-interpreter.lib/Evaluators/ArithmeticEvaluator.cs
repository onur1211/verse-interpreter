using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Evaluators
{
    public class ArithmeticEvaluator : IEvaluator<int, List<List<ExpressionResult>>>
    {
        public int Evaluate(List<List<ExpressionResult>> input)
        {
            int result = 0;
            input.Reverse();
            foreach (var item in input)
            {
                result += EvaluateSubExpression(item);
            }

            return result;
        }

        private int EvaluateSubExpression(List<ExpressionResult> expression)
        {
            return 0;
        }
    }
}
