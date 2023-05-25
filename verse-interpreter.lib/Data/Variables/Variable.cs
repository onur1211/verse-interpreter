
﻿using System.Threading.Tasks.Sources;
using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data
{
    public class Variable
    {
        public Variable(string name, ValueObject value) 
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; } = null!;

        public ValueObject Value { get; set; }   

        public bool HasValue()
        {
            return Value.IntValue != null || Value.StringValue != null || Value.DynamicType != null || Value.CollectionVariable != null;
        }
    }
}
