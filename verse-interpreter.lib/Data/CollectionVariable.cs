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
        public CollectionVariable(string name, string type, Variable[] values) : base(name, type)
        {
            this.Type = type;
            this.Values = values;
        }

        public Variable[] Values { get; set; }

        public override Variable[] AcceptCollection(IVariableVisitor visitor)
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

        private void CheckValueTypes(Variable[] variables)
        {
            // Check the type of each variable in the array to
            // ensure that only variables of the same type are in
            // the collection.
            foreach (var variable in variables) 
            {
                if (variable.Type != this.Type) 
                {
                    throw new InvalidTypeException(variable.Type);
                }
            }
        }
    }
}
