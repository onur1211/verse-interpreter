using verse_interpreter.lib.Data;

namespace verse_interpreter.lib.Factories
{
    public class ArithmeticExpression 
    {
        public string StringRepresentation { get; set; } = null!;

        public Nullable<int> ResultValue { get; set; }
    }
}