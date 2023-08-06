namespace verse_interpreter.lib.Data.ResultObjects
{
    [Serializable]
    public class ExpressionResult
    {
        public Nullable<int> IntegerValue { get; set; }

        public string ValueIdentifier { get; set; }

        public string Operator { get; set; }
        public string StringValue { get; internal set; }

        public string TypeName { get; set; }

        public ExpressionResult()
        {
            IntegerValue = null;
            ValueIdentifier = string.Empty;
            Operator = string.Empty;
        }
    }
}