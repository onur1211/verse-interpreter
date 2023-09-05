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

		public Choice()
		{
			ValueObject = null!;
		}

		public void AddValue(int? value)
		{
			if (value == null)
			{
				return;
			}
			if (ValueObject == null)
			{
				ValueObject = new ValueObject("int");
			}

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

		public void AddValue(string? value)
		{
			if (value == null)
			{
				return;
			}
			if (ValueObject == null)
			{
				ValueObject = new ValueObject("string");
			}

			Choice choice = new Choice(new ValueObject("string", value));
			if (ValueObject.StringValue == null)
			{
				ValueObject.StringValue = value;
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

			while (current.Next != null)
			{
				current = current.Next;
				yield return current;
			}
		}
	}
}
