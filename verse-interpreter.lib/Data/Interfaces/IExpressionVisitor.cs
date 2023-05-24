using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Factories;

namespace verse_interpreter.lib.Data.Interfaces
{
    public interface IExpressionVisitor
    {
        void Visit(IExpression<ArithmeticExpression> expression);

        void Visit(IExpression<StringExpressionResolvedEventArgs> expression);
    }
}
