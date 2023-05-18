using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Variables;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib.Converter
{
    public static class VariableConverter
    {
        public static Variable Convert(DeclarationResult declarationResult, ApplicationState state)
        {
            // Pattern match the type. # functional programming
            // object oriented programming sucks
            return declarationResult.TypeName switch
            {
                "int" => HandleIntVariables(declarationResult),
                "string" => new StringVariable(declarationResult.Name, declarationResult.TypeName, declarationResult.Value),
                _ => HandleDynamicType(declarationResult, state)
            };
        }

        private static IntVariable HandleIntVariables(DeclarationResult declarationResult)
        {
            if (declarationResult.Value == null)
            {
                return new IntVariable(declarationResult.Name, declarationResult.TypeName, null);
            }
            else
            {
                return new IntVariable(declarationResult.Name, declarationResult.TypeName, int.Parse(declarationResult.Value));
            }
        }

        private static DynamicVariable HandleDynamicType(DeclarationResult declarationResult, ApplicationState state)
        {
            if (!state.Types.ContainsKey(declarationResult.TypeName))
            {
                throw new UnknownTypeException(declarationResult.TypeName);
            }

            return new DynamicVariable(declarationResult.Name, declarationResult.TypeName, declarationResult.DynamicType!);
        }
    }
}
