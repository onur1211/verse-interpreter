using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.Variables;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Extensions;

namespace verse_interpreter.lib.Converter
{
	public static class VariableConverter
	{
		public static Variable Convert(DeclarationResult declarationResult)
		{
			return declarationResult.TypeName switch
			{
				"int" => HandleIntVariables(declarationResult),
				"string" => HandleStringVariable(declarationResult),
				"int[]" => HandleExplicitCollectionVariables(declarationResult, "int"),
				"string[]" => HandleExplicitCollectionVariables(declarationResult, "string"),
				"false?" => new Variable(declarationResult.Name, ValueObject.False),
				"collection" => new Variable(declarationResult.Name, new(declarationResult.TypeName, declarationResult.CollectionVariable)),
				"undefined" => new Variable(declarationResult.Name, new("undefined")),
				_ => HandleCustomType(declarationResult)
			};
		}

		public static Variable Convert(ChoiceResult choice)
		{
			Choice finishedChoice = new Choice();
			var current = choice;
			if (current.Next == null)
			{
				AddValuesToChoice(finishedChoice, current);
			}

			while (current.Next != null)
			{
				AddValuesToChoice(finishedChoice, current);

				current = current.Next;
				if (current.Next == null)
				{
					AddValuesToChoice(finishedChoice, current);
				}
			}

			return new Variable()
			{
				Value = new ValueObject(finishedChoice.ValueObject.TypeData.Name)
				{
					Choice = finishedChoice,
				}
			};
		}

		private static void AddValuesToChoice(Choice choice, ChoiceResult choiceResult)
		{
			if (choiceResult.Literals.First().Value.StringValue != null)
			{
				choice.AddValue(choiceResult.Literals.First().Value.StringValue);
			}
			if (choiceResult.Literals.First().Value.IntValue != null)
			{
				choice.AddValue(choiceResult.Literals.First().Value.IntValue);
			}
			if (choiceResult.IndexingResults.FirstOrDefault() != null)
			{
				throw new NotImplementedException("A choice using indexing results is currently not supported");
			}
		}

		private static Variable HandleIntVariables(DeclarationResult declarationResult)
		{
			if (declarationResult.ChoiceResult != null)
			{
				var variable = Convert(declarationResult.ChoiceResult);
				variable.Name = declarationResult.Name;
				return variable;
			}

			if (declarationResult.IndexedVariable != null && declarationResult.Name == null)
			{
				return declarationResult.IndexedVariable;
			}
			if (string.IsNullOrEmpty(declarationResult.LiteralValue))
			{
				return new Variable(declarationResult.Name, new ValueObject(declarationResult.TypeName));
			}
			else
			{
				return new Variable(declarationResult.Name, new ValueObject(declarationResult.TypeName, int.Parse(declarationResult.LiteralValue)));
			}
		}

		private static Variable HandleStringVariable(DeclarationResult declarationResult)
		{
			if (declarationResult.ChoiceResult != null)
			{
				var variable = Convert(declarationResult.ChoiceResult);
				variable.Name = declarationResult.Name;
				return variable;
			}
			if (declarationResult.IndexedVariable != null && declarationResult.Name == null)
			{
				return declarationResult.IndexedVariable;
			}

			return new Variable()
			{
				Name = declarationResult.Name,
				Value = new ValueObject(declarationResult.TypeName, declarationResult.LiteralValue)
			};
		}


		public static DeclarationResult ConvertBack(Variable variable)
		{
			if (variable.Value.Choice != null)
			{
				return new DeclarationResult()
				{
					Name = variable.Name,
					ChoiceResult = ConvertChoiceToResult(variable.Value.Choice),
					TypeName = variable.Value.TypeData.Name
				};
			}

			switch (variable.Value.TypeData.Name)
			{
				case "int":
					return new DeclarationResult()
					{
						Name = variable.Name,
						CustomType = variable.Value.CustomType,
						LiteralValue = variable.Value.IntValue.ToString(),
						TypeName = variable.Value.TypeData.Name,
					};

				case "string":
					return new DeclarationResult()
					{
						Name = variable.Name,
						CustomType = variable.Value.CustomType,
						LiteralValue = variable.Value.StringValue,
						TypeName = variable.Value.TypeData.Name,
						CollectionVariable = variable.Value.CollectionVariable,
					};

				default:
					return new DeclarationResult()
					{
						Name = variable.Name,
						CustomType = variable.Value.CustomType,
						CollectionVariable = variable.Value.CollectionVariable,
						TypeName = variable.Value.TypeData.Name,
					};
			}
		}

		private static ChoiceResult ConvertChoiceToResult(Choice choice)
		{
			ChoiceResult choiceResult = new ChoiceResult();
			var current = choiceResult;
			var last = choice.AllChoices().Last();
			foreach (var element in choice.AllChoices())
			{
				while (current.Next != null)
				{
					current = current.Next;
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

			return choiceResult;
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
			return new Variable(declarationResult.Name, new(declarationResult.TypeName, declarationResult.CustomType.Value!));
		}
	}
}
