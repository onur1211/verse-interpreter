namespace verse_interpreter.lib.Data
{
    public class ValueObject : IComparable<ValueObject>
    {
        public ValueObject(string typeName)
        {
            this.TypeName = typeName;    
        }

        public ValueObject(string typeName, string stringValue)
        {
            this.TypeName = typeName;
            this.StringValue = stringValue;
        }

        public ValueObject(string typeName, int? intValue)
        {
            this.TypeName = typeName;
            this.IntValue = intValue;
        }

        public ValueObject(string typeName, DynamicType dynamicType)
        {
            this.TypeName = typeName;
            this.DynamicType = dynamicType;
        }

        public ValueObject(string typeName, VerseCollection collectionVariable) : this(typeName)
        {
            CollectionVariable = collectionVariable;
        }

        public string TypeName { get; set; }

        public string StringValue { get; set; } = null!;

        public int? IntValue { get; set; } = null;

        public DynamicType DynamicType { get; set; } = null!;

        public VerseCollection CollectionVariable { get; set; } = null!;

        public int CompareTo(ValueObject? other)
        {
            throw new NotImplementedException();
        }
    }
}