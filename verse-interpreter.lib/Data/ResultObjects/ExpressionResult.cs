namespace verse_interpreter.lib.Data.ResultObjects
{
    public class ExpressionResult
    {
        public Nullable<int> IntegerValue { get; set; }

        public string StringValue { get; set; }

        public string ValueIdentifier { get; set; }

        public string Operator { get; set; }

        public ExpressionResult()
        {
            IntegerValue = null;
            StringValue = null;
            ValueIdentifier = string.Empty;
            Operator = string.Empty;
        }


        public List<List<ExpressionResult>> Values { get; set; }
    }
}