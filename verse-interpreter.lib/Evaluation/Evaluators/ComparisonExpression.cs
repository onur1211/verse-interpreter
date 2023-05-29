using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.ResultObjects;

namespace verse_interpreter.lib.Evaluation.Evaluators
{
    public class ComparisonExpression : IExpression<ComparisonExpression>
    {
        public Func<ComparisonExpression>? PostponedExpression { get; set; }

        public int? Value { get; set; }

        public List<List<ExpressionResult>> Arguments { get; set; }
    }
}
