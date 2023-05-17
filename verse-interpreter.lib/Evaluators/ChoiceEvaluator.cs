using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.DataObjects;

namespace verse_interpreter.lib.Evaluators
{
    public class ChoiceEvaluator<T> : IEvaluator<IEnumerable<T>, Choice<T>>
    {
        public IEnumerable<T> Evaluate(Choice<T> input)
        {
            throw new NotImplementedException();
        }
    }
}
