
using System.Reflection;
using System.Threading.Tasks.Sources;
using verse_interpreter.lib.Data.Interfaces;

namespace verse_interpreter.lib.Data
{
	public class Variable : IUnifiable<Variable>
	{
		private static Variable _false;

		public Variable(string name, ValueObject value)
		{
			this.Name = name;
			this.Value = value;
		}

		public Variable()
		{

		}

		public static Variable False
		{
			get
			{
				if (_false == null)
				{
					_false = new Variable()
					{
						Value = new ValueObject("false?")
					};
				}

				return _false;
			}
		}

		public string Name { get; set; } = null!;

		public ValueObject Value { get; set; }

		public bool CanUnify(Variable variable)
		{
			return variable.Value.Equals(Value);
		}

		public bool HasValue()
		{
			return Value.IntValue != null || Value.StringValue != null || Value.CustomType.HasValue || Value.CollectionVariable != null;
		}
	}
}
