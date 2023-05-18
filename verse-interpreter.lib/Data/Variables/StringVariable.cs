using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data.Variables
{
    public class StringVariable : Variable
    {
        public StringVariable(string name, string type, string value) : base(name, type)
        {
            this.Value = value;
        }

        public string Value { get; set; }

        public override DynamicType AcceptDynamicType(IVariableVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override int? AcceptInt(IVariableVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override string AcceptString(IVariableVisitor visitor)
        {
            return this.Value;
        }

        public override bool HasValue()
        {
            return Value != null;
        }
    }
}
