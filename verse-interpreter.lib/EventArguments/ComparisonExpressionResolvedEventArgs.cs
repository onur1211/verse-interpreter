using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Evaluation.Evaluators;

namespace verse_interpreter.lib.Evaluation.EvaluationManagement
{
    public class ComparisonExpressionResolvedEventArgs : EventArgs
    {
        public ComparisonExpression Result { get; }

        public ComparisonExpressionResolvedEventArgs(ComparisonExpression res)
        {
            Result = res;
        }
    }
}
