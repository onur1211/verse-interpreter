using verse_interpreter.lib.Data.Variables;

namespace verse_interpreter.lib.Data
{
	public class ValueObject
    {
        public ValueObject(string typeName)
        {
            this.TypeData = new TypeData(typeName);    
        }

        public ValueObject(string typeName, string stringValue) : this(typeName)
		{
			this.StringValue = stringValue;
        }

        public ValueObject(string typeName, int? intValue) : this(typeName)
		{
            this.IntValue = intValue;
        }

        public ValueObject(string typeName, DynamicType dynamicType) : this(typeName)
        {
            this.DynamicType = dynamicType;
        }

        public ValueObject(string typeName, VerseCollection collectionVariable) : this(typeName)
        {
            this.CollectionVariable = collectionVariable;
        }

        public TypeData TypeData { get; set; }

        public string StringValue { get; set; } = null!;

        public int? IntValue { get; set; } = null;

        public DynamicType? DynamicType { get; set; } = null!;

        public VerseCollection CollectionVariable { get; set; } = null!;
	}
}