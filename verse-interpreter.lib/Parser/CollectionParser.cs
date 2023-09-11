using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Parser
{
	public class CollectionParser
	{
		public CollectionParseResult GetParameters(Verse.Array_elementsContext context)
		{
			CollectionParseResult collectionParseResult = new CollectionParseResult();
			int indexCounter = 0;
			collectionParseResult = this.ParseParameterRecursive(context, collectionParseResult, indexCounter);
			collectionParseResult.TotalElements = collectionParseResult.ValueElements.Count + collectionParseResult.DeclarationElements.Count + collectionParseResult.VariableElements.Count;
			return collectionParseResult;
		}

		private CollectionParseResult ParseParameterRecursive(Verse.Array_elementsContext context, CollectionParseResult collectionParseResult, int indexCounter)
		{
			if (context == null)
			{
				// Done no children left. Return the collection parse result.
				return collectionParseResult;
			}

			var valueDefs = context.value_definition();
			var declDefs = context.declaration();
			var variableDefs = context.ID();

			if (valueDefs != null)
			{
				collectionParseResult.ValueElements.Add(indexCounter, valueDefs);
				indexCounter++;
			}

			if (declDefs != null)
			{
				collectionParseResult.DeclarationElements.Add(indexCounter, declDefs);
				indexCounter++;
			}

			if (variableDefs != null)
			{
				collectionParseResult.VariableElements.Add(indexCounter, variableDefs.GetText());
				indexCounter++;
			}

			// Go to the next child recursive.
			return ParseParameterRecursive(context.array_elements().FirstOrDefault()!, collectionParseResult, indexCounter);
		}
	}
}
