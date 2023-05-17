using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data.Variables
{
    public class IntVariable : Variable, IVariableVisitable<int?>
    {
        public IntVariable(string name, string type, int? value) : base(name, type)
        {
            this.Value = value;
        }

        public int? Value { get; set; }

        public int? Accept(IVariableVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override T Accept<T>(IVariableVisitor variableVisitor)
        {
            throw new NotImplementedException();
        }
    }
}
