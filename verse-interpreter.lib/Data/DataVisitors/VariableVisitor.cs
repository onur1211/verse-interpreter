using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.Variables;

namespace verse_interpreter.lib.Data.DataVisitors
{
    public class VariableVisitor : IVariableVisitor
    {
        public int? Visit(IntVariable variable)
        {
            return variable.Value;
        }

        public string Visit(StringVariable variable)
        {
            return variable.Value;
        }

        public DynamicType Visit(DynamicVariable variable)
        {
            return variable.Value;
        }
    }
}
