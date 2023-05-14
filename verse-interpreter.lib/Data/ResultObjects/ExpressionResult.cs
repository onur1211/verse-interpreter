namespace verse_interpreter.lib.Data.ResultObjects
{
    public class ExpressionResult
    {
        public Nullable<int> Value { get; set; }

        public string ValueIdentifier { get; set; }

        public string Operator { get; set; }

        public ExpressionResult()
        {
            Value = null;
            ValueIdentifier = string.Empty;
            Operator = string.Empty;
        }

    }
}