using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data.Variables
{
    public class DynamicVariable : Variable, IVariableVisitable<DynamicType>
    {
        public DynamicVariable(string name, string type, DynamicType value) : base(name, type) 
        {
            this.Value = value;
        }

        public DynamicType Value { get; set; }

        public DynamicType Accept(IVariableVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override T Accept<T>(IVariableVisitor variableVisitor)
        {
            throw new NotImplementedException();
        }
    }
}
