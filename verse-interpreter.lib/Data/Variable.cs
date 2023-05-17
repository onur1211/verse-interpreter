using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data
{
    public abstract class Variable
    {
        public Variable(string name, string type) 
        {
            this.Name = name;
            this.Type = type;
        }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public abstract T Accept<T>(IVariableVisitor variableVisitor);
    }
}
