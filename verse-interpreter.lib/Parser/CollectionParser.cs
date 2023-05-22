using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Parser
{
    public class CollectionParser
    {
        public CollectionParseResult GetParameters(Verse.Array_elementsContext context)
        {
            CollectionParseResult collectionParseResult = new CollectionParseResult();

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return this.ParseParameterRecursive(context, collectionParseResult);
        }

        private CollectionParseResult ParseParameterRecursive(Verse.Array_elementsContext context, CollectionParseResult collectionParseResult)
        {
            if (context == null) 
            {
                // Done no children left. Return the collection parse result.
                return collectionParseResult;
            }

            var valueDefs = context.value_definition();
            var declDefs = context.declaration();

            if (valueDefs != null) 
            {
                collectionParseResult.ValueElements.Add(valueDefs);
            }

            if (declDefs != null) 
            {
                collectionParseResult.DeclarationElements.Add(declDefs);
            }

            // Go to the next child recursive.
            return ParseParameterRecursive(context.array_elements().FirstOrDefault(), collectionParseResult);
        }
    }
}
