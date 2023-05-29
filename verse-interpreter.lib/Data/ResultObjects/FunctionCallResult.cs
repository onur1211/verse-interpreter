using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Factories;

namespace verse_interpreter.lib.Data.ResultObjects
{
    public class FunctionCallResult
    {
        public ArithmeticExpression ArithmeticExpression { get; set; }

        public StringExpression StringExpression { get; set; }
    }
}
