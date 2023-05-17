using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Data.Interfaces
{
    public interface IVariableVisitable<T>
    {
        T Accept(IVariableVisitor visitor);
    }
}
