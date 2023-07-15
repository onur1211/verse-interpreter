using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib.Converter
{
    public static class VariableConverter
    {
        public static Variable Convert(DeclarationResult declarationResult)
        {
            // Pattern match the type. functional programming <3
            // => object oriented programming sucks
            return declarationResult.TypeName switch
            {
                "int" => HandleIntVariables(declarationResult),
                "string" => new Variable(declarationResult.Name, new(declarationResult.TypeName, declarationResult.Value)),
                "collection" => new Variable(declarationResult.Name, new(declarationResult.TypeName, declarationResult.CollectionVariable)),
                _ => HandleCustomType(declarationResult)
            };
        }

        private static Variable HandleIntVariables(DeclarationResult declarationResult)
        {
            if (declarationResult.Value == null)
            {
                return new Variable(declarationResult.Name, new ValueObject(declarationResult.TypeName));
            }
            else
            {
                return new Variable(declarationResult.Name, new ValueObject(declarationResult.TypeName, int.Parse(declarationResult.Value)));
            }
        }

        private static Variable HandleCustomType(DeclarationResult declarationResult)
        {
            return new Variable(declarationResult.Name, new(declarationResult.TypeName, declarationResult.CustomType!));
        }
    }
}
