using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.Variables;

namespace verse_interpreter.lib.Converter
{
	public static class ChoiceConverter
	{
		public static Choice Convert(VerseCollection collection)
		{
			Choice choice = new Choice();
			foreach (var element in collection.Values)
			{
				choice.AddValue(element.Value.IntValue);
				choice.AddValue(element.Value.StringValue);
			}

			return choice;
		}

		public static ChoiceResult Convert(Choice choice)
		{
			ChoiceResult result = new ChoiceResult();
			var current = result;
			var last = choice.AllChoices().LastOrDefault();
			foreach (var element in choice.AllChoices())
			{
				while (current.Next != null)
				{
					current = current.Next;
				}
				if (element.ValueObject == null)
				{
					continue;
				}

				current.Literals.Add(new Variable()
				{
					Value = element.ValueObject
				});
				if (element != last)
				{
					current.Next = new ChoiceResult();
				}
			}

			return result;
		}
	}
}
