using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data
{
    public abstract class Variable : IVariableVisitable
    {
        public Variable(string name, string type) 
        {
            this.Name = name;
            this.Type = type;
        }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public abstract DynamicType AcceptDynamicType(IVariableVisitor visitor);

        public abstract int? AcceptInt(IVariableVisitor visitor);

        public abstract string AcceptString(IVariableVisitor visitor);

        public abstract bool HasValue();
    }
}
