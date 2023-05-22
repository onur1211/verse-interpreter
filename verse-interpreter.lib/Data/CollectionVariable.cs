using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib.Data
{
    public class CollectionVariable : Variable
    {
        public CollectionVariable(string name, string type, List<Variable> values) : base(name, type)
        {
            this.Type = type;
            this.Values = values;
        }

        public List<Variable> Values { get; set; }

        public override List<Variable> AcceptCollection(IVariableVisitor visitor)
        {
            return this.Values;
        }

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
            throw new NotImplementedException();
        }

        public override bool HasValue()
        {
            return this.Values.Count > 0;
        }
    }
}
