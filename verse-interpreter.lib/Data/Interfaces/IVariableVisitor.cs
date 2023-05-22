using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Variables;

namespace verse_interpreter.lib.Data.Interfaces
{
    public interface IVariableVisitor
    {
        int? Visit(IntVariable variable);

        string Visit(StringVariable variable);

        DynamicType Visit(DynamicVariable variable);

        List<Variable> Visit(CollectionVariable variable);
    }
}
