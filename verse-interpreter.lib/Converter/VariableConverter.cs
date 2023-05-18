using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Variables;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib.Converter
{
    public static class VariableConverter
    {
        public static Variable Convert(DeclarationResult declarationResult)
        {
            // Pattern match the type. # functional programming
            // object oriented programming sucks
            return declarationResult.TypeName switch
            {
                "int" => HandleIntVariables(declarationResult),
                "string" => new StringVariable(declarationResult.Name, declarationResult.TypeName, declarationResult.Value),
                "dynamic" => new DynamicVariable(declarationResult.Name, declarationResult.TypeName, declarationResult.DynamicType!),
                "collection" => declarationResult.CollectionVariable!,
                _ => throw new UnknownTypeException(declarationResult.TypeName),
            }; 
        }

        private static IntVariable HandleIntVariables(DeclarationResult declarationResult)
        {
            if(declarationResult.Value ==  null)
            {
                return new IntVariable(declarationResult.Name, declarationResult.TypeName, null);
            }
            else
            {
                return new IntVariable(declarationResult.Name, declarationResult.TypeName, int.Parse(declarationResult.Value));
            }
        }
    }
}
