using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Data.ResultObjects.Expressions
{
	public class ExpressionSet
	{
		public List<List<ExpressionResult>> Expressions { get; set; } = null!;

        public ExpressionSet(List<List<ExpressionResult>> expressionResults)
        {
                this.Expressions = expressionResults;
        }
    }
}
