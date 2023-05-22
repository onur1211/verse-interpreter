using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.DataVisitors;

namespace verse_interpreter.lib.Data.Interfaces
{
    public interface IVariableVisitable
    {
        string AcceptString(IVariableVisitor visitor); 

        int? AcceptInt(IVariableVisitor visitor);

        DynamicType AcceptDynamicType(IVariableVisitor visitor);

        List<Variable> AcceptCollection(IVariableVisitor visitor);
    }
}
