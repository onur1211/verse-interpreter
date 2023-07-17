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
				"int[]" => HandleExplicitCollectionVariables(declarationResult, "int"),
				"string[]" => HandleExplicitCollectionVariables(declarationResult, "string"),
				"collection" => new Variable(declarationResult.Name, new(declarationResult.TypeName, declarationResult.CollectionVariable)),
				_ => HandleCustomType(declarationResult)
			};
		}
		private static Variable HandleIntVariables(DeclarationResult declarationResult)
		{
			if (declarationResult.IndexedVariable != null && declarationResult.Name == null)
			{
				return declarationResult.IndexedVariable;
			}
			if (declarationResult.Value == null)
			{
				return new Variable(declarationResult.Name, new ValueObject(declarationResult.TypeName));
			}
			else
			{
				return new Variable(declarationResult.Name, new ValueObject(declarationResult.TypeName, int.Parse(declarationResult.Value)));
			}
		}
        public static DeclarationResult ConvertBack(Variable variable)
        {
            switch (variable.Value.TypeData.Name)
            {
                case "int":
                    return new DeclarationResult()
                    {
                        CustomType = variable.Value.CustomType,
                        Value = variable.Value.IntValue.ToString(),
                        TypeName = variable.Value.TypeData.Name,
                    };

                case "string":
                    return new DeclarationResult()
                    {
                        CustomType = variable.Value.CustomType,
                        Value = variable.Value.StringValue,
                        TypeName = variable.Value.TypeData.Name,
                    };

                default:
                    return new DeclarationResult()
                    {
                        CustomType = variable.Value.CustomType,
                        CollectionVariable = variable.Value.CollectionVariable,
                        TypeName = variable.Value.TypeData.Name,
                    };
            }
        }

		private static Variable HandleExplicitCollectionVariables(DeclarationResult declarationResult, string elementsInCollectionType)
		{
			// Currently the expliciti collection types like int[] or string[] are just converted and handled as normal collections.
			declarationResult.TypeName = "collection";
			Variable variable;
			if (declarationResult.CollectionVariable == null)
			{
				variable = new Variable(declarationResult.Name, new(declarationResult.TypeName, new VerseCollection(new List<Variable>())));
				variable.Value.CollectionVariable.TypeData.Name = elementsInCollectionType;
				return variable;
			}

			variable = new Variable(declarationResult.Name, new(declarationResult.TypeName, declarationResult.CollectionVariable));
			variable.Value.CollectionVariable.TypeData.Name = elementsInCollectionType;

			return variable;
		}

		private static Variable HandleCustomType(DeclarationResult declarationResult)
		{
			return new Variable(declarationResult.Name, new(declarationResult.TypeName, declarationResult.CustomType!));
		}
	}
}
