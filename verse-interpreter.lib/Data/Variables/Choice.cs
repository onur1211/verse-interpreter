namespace verse_interpreter.lib.Data.Variables
{
    public class Choice
    {
        public ValueObject ValueObject { get; set; }

        public Choice? Next { get; set; }

        public TypeData Type
        {
            get
            {
                return this.ValueObject.TypeData;
            }
        }

        public Choice(ValueObject valueObject)
        {
            ValueObject = valueObject;
        }

        public Choice()
        {
            ValueObject = null!;
        }

        public void AddValue(int? value)
        {
            var current = this;
            if (value == null)
            {
                return;
            }

            while (current.Next != null)
            {
                current = current.Next;
            }

            if (current.ValueObject == null)
            {
                current.ValueObject = new ValueObject("int");
            }
            if (current.ValueObject.IntValue == null &&
                current.ValueObject.StringValue == null)
            {
                current.ValueObject.IntValue = value;
                return;
            }

            if (current.Next == null)
            {
                current.Next = new Choice(new ValueObject("int"));
                current = current.Next;
            }
            while (current.Next != null)
            {
                current = current.Next;
            }

            current.ValueObject = new ValueObject("int");
            current.ValueObject.IntValue = value;
        }

        public void AddValue(string? value)
        {
            var current = this;
            if (value == null)
            {
                return;
            }

            while (current.Next != null)
            {
                current = current.Next;
            }

            if (current.ValueObject == null)
            {
                current.ValueObject = new ValueObject("string");
            }
            if (current.ValueObject.IntValue == null &&
                current.ValueObject.StringValue == null)
            {
                current.ValueObject.StringValue = value;
                return;
            }

            if (current.Next == null)
            {
                current.Next = new Choice(new ValueObject("string"));
                current = current.Next;
            }
            while (current.Next != null)
            {
                current = current.Next;
            }

            current.ValueObject = new ValueObject("string");
            current.ValueObject.StringValue = value;
        }

        public IEnumerable<Choice> AllChoices()
        {
            var current = this;
            yield return current;

            while (current.Next != null)
            {
                current = current.Next;
                yield return current;
            }
        }
    }
}
