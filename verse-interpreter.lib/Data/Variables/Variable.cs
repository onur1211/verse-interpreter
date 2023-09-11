namespace verse_interpreter.lib.Data
{
	public class Variable : IUnifiable<Variable>
	{
		public Variable(string name, ValueObject value)
		{
			this.Name = name;
			this.Value = value;
		}

		public Variable()
		{
			this.Value = null!;
		}

		public string Name { get; set; } = null!;

		public ValueObject Value { get; set; }

		public bool CanUnify(Variable variable)
		{
			return variable.Value.Equals(Value);
		}

		public bool HasValue()
		{
			return Value.IntValue != null ||
				Value.StringValue != null ||
				Value.CustomType.HasValue ||
				Value.CollectionVariable != null ||
				Value.Choice != null;
		}
	}
}
