using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data.Variables
{
    public class IntVariable : Variable
    {
        public IntVariable(string name, string type, int? value) : base(name, type)
        {
            this.Value = value;
        }

        public int? Value { get; set; }

        public override List<Variable> AcceptCollection(IVariableVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override DynamicType AcceptDynamicType(IVariableVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override int? AcceptInt(IVariableVisitor visitor)
        {
            return Value;
        }

        public override string AcceptString(IVariableVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
