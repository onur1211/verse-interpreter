using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.DataVisitors;
using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data.Variables
{
    public class DynamicVariable : Variable
    {
        public DynamicVariable(string name, string type, DynamicType value) : base(name, type) 
        {
            this.Value = value;
        }

        public DynamicType Value { get; set; }

        public override Variable[] AcceptCollection(IVariableVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override DynamicType AcceptDynamicType(IVariableVisitor visitor)
        {
            return Value;
        }

        public override int? AcceptInt(IVariableVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override string AcceptString(IVariableVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
