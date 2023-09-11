using verse_interpreter.lib.Data.CustomTypes;
using verse_interpreter.lib.Data.Variables;

namespace verse_interpreter.lib.Data
{
    public class ValueObject
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private static ValueObject _false;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

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

        private static int counter;

        public static ValueObject False
        {
            get
            {
                if (_false == null)
                {
                    _false = new ValueObject("false?");
                }
                counter += 1;
                return _false;
            }
        }
    }
}