using verse_interpreter.lib.Data.CustomTypes;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.Variables;

namespace verse_interpreter.lib.Data
{
	public class ValueObject
	{
		private static ValueObject _false;

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

		public ValueObject(string typeName, CustomType customType) : this(typeName)
		{
			this.CustomType = customType;
		}

		public ValueObject(string typeName, VerseCollection collectionVariable) : this(typeName)
		{
			this.CollectionVariable = collectionVariable;
		}

		public ValueObject(string typeName, Choice choiceResult) : this(typeName)
		{
			this.Choice = choiceResult;
		}

		public TypeData TypeData { get; set; }

		public string StringValue { get; set; } = null!;

		public int? IntValue { get; set; } = null;

		public CustomType? CustomType { get; set; } = null!;

		public VerseCollection CollectionVariable { get; set; } = null!;

		public Choice Choice { get; set; } = null!;

		public static ValueObject False
		{
			get
			{
				if (_false == null)
				{
					_false = new ValueObject("false?");
				}

				return _false;
			}
		}
	}
}