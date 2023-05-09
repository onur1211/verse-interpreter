using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Data
{
    public interface IScope<T>
    {
        List<Variable<T>> Variables { get; }

        void AddVariable(Variable<T> variable);
    }
}
