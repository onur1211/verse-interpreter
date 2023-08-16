using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public void AddValue(int value)
		{
			Choice choice = new Choice(new ValueObject("int", value));
			if (ValueObject.IntValue == null)
			{
				ValueObject.IntValue = value;
				return;
			}

			choice.Next = null;

			Choice temp = this;
			while (temp.Next != null)
			{
				temp = temp.Next;
			}
			temp.Next = choice;
		}

		public IEnumerable<Choice> AllChoices()
		{
			var current = this;
			yield return current;

			if (current.Next == null)
			{
				yield return current;
			}

			while (current.Next != null)
			{
				current = current.Next;
				yield return current;
			}
		}
	}
}
