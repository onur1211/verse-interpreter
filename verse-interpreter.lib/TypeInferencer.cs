using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib
{
    // EXAM_UPDATED
    public class TypeInferencer
    {
        public DeclarationResult InferGivenType(DeclarationResult declarationResult)
        {
            string typeName;

            if (declarationResult == null)
            {
                throw new ArgumentNullException("The specified input object is null!");
            }

            if (declarationResult.TypeName == "undefined")
            {
                var isInt = int.TryParse(declarationResult.Value, out _);
                if (isInt)
                {
                    declarationResult.TypeName = "int";
                }
                else
                {
                    declarationResult.TypeName = "string";
                }
            }

            return declarationResult;
        }

        public DeclarationResult InferGivenType(Verse.DeclarationContext context)
        {
            DeclarationResult result = new DeclarationResult();

            result.Name = context.ID().GetText();
            var unparsedInt = context.INT();
            var unparsedString = context.string_rule();

            if(unparsedInt != null)
            {
                result.Value = unparsedInt.GetText();
                result.TypeName = "int";
            }
            if(unparsedString != null)
            {
                result.Value = unparsedString.GetText();
                result.TypeName = "string";
            }

            return result;
        }
    }
}
