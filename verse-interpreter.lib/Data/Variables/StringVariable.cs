using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data.Variables
{
    public class StringVariable : Variable, IVariableVisitable<string>
    {
        public StringVariable(string name, string type, string value) : base(name, type)
        {
            this.Value = value;
        }

        public string Value { get; set; }

        public string Accept(IVariableVisitor visitor)
        {
            return this.Accept(visitor);
        }

        public override T Accept<T>(IVariableVisitor variableVisitor)
        {
            throw new NotImplementedException();
        }
    }
}
