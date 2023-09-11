using verse_interpreter.lib.Data.Variables;

namespace verse_interpreter.lib.Data
{
    public class VerseCollection
    {
        public VerseCollection(List<Variable> values)
        {
            this.Values = values;
            TypeData = new TypeData("any");
        }

        public TypeData TypeData { get; set; }

        public List<Variable> Values { get; set; }

        public bool HasValue()
        {
            return this.Values.Count > 0;
        }
    }
}
