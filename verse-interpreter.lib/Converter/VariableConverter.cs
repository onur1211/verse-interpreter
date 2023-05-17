using System;
using System.Collections.Generic;
using System.Linq;
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
                "int" => new IntVariable(declarationResult.Name, declarationResult.TypeName, int.Parse(declarationResult.Value)),
                "string" => new StringVariable(declarationResult.Name, declarationResult.TypeName, declarationResult.Value),
                "dynamic" => new DynamicVariable(declarationResult.Name, declarationResult.TypeName, declarationResult.DynamicType!),
                _ => throw new UnknownTypeException(declarationResult.TypeName),
            };
        }
    }
}
