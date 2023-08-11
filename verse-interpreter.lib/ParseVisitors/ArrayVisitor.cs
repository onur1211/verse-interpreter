using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.lib.ParseVisitors
{
	public class ArrayVisitor : AbstractVerseVisitor<DeclarationResult>
	{
		private readonly Lazy<PropertyResolver> _resolver;
		private readonly Lazy<TypeInferencer> _typeInferencer;
		private readonly Lazy<CollectionParser> _collectionParser;
		private readonly Lazy<DeclarationParser> _declarationParser;
		private readonly Lazy<ValueDefinitionVisitor> _valueDefinitionVisitor;

		public ArrayVisitor(ApplicationState applicationState,
									Lazy<PropertyResolver> propertyResolver,
									Lazy<TypeInferencer> typeInferencer,
									Lazy<CollectionParser> collectionParser,
									Lazy<DeclarationParser> declarationParser,
									Lazy<ValueDefinitionVisitor> valueDefinitionVisitor) : base(applicationState)
		{
			_resolver = propertyResolver;
			_typeInferencer = typeInferencer;
			_collectionParser = collectionParser;
			_declarationParser = declarationParser;
			_valueDefinitionVisitor = valueDefinitionVisitor;
		}

		public override DeclarationResult VisitArray_literal([NotNull] Verse.Array_literalContext context)
		{
			List<Variable> variables = new List<Variable>();
			DeclarationResult rangeExpressionResult = new DeclarationResult();
			var result = _collectionParser.Value.GetParameters(context.array_elements());
			// Check if there are value elements in the collection
			// Example: myArray:=(1,2,3) => 1,2 and 3 are value elements
			if (result.ValueElements != null)
			{
				foreach (var valueDef in result.ValueElements)
				{
					// Accept the range expression
					rangeExpressionResult = valueDef.Accept(_valueDefinitionVisitor.Value)!;

					// Check if there is a range expression
					if (rangeExpressionResult.CollectionVariable != null)
					{
						continue;
					}

					var variableResult = VariableConverter.Convert(rangeExpressionResult);
					variables.Add(variableResult);
				}
			}

			// Check if there are declaration elements in the collection
			// Example: myArray:=(x:=1,2) => x:=1 is a declaration element
			if (result.DeclarationElements != null)
			{
				foreach (var declDef in result.DeclarationElements)
				{
					var variableResult = _declarationParser.Value.ParseDeclaration(declDef);
					variables.Add(VariableConverter.Convert(variableResult));
				}
			}

			// Check if there are variable elements in the collection
			// Example: x:=1; y:=2; myArray:=(x,y) => x and y are variable elements
			if (result.VariableElements != null)
			{
				foreach (var variable in result.VariableElements)
				{
					var variableResult = ApplicationState.CurrentScope.LookupManager.GetVariable(variable);
					variables.Add(variableResult);
				}
			}

			DeclarationResult declarationResult = new DeclarationResult();
			declarationResult.CollectionVariable = new VerseCollection(variables);

			if (rangeExpressionResult.CollectionVariable != null)
			{
				declarationResult = rangeExpressionResult;
			}

			return _typeInferencer.Value.InferGivenType(declarationResult);
		}

		public override DeclarationResult VisitDefaultIndexing([NotNull] Verse.DefaultIndexingContext context)
		{
			string index = String.Empty;
			// Check if the given index is a number
			// Example: myArray[0] -> 0
			if (context.INT() != null)
			{
				index = context.INT().GetText();

				if (index == null || index == String.Empty)
				{
					throw new ArgumentNullException(nameof(index), "Error: There was no index given for the array access.");
				}
			}

			// Check if the given index is a variable
			// Example: myArray[x] -> x
			if (context.ID().Length > 1)
			{
				// Get the name of the variable
				string variableName = context.ID().ElementAt(1).GetText();

				if (variableName == null || variableName == String.Empty)
				{
					throw new ArgumentNullException(nameof(index), "Error: There was no variable as index given for the array access.");
				}

				if (!ApplicationState.CurrentScope.LookupManager.IsVariable(variableName))
				{
					throw new VariableDoesNotExistException(nameof(variableName));
				}

				// Get the actual variable
				Variable variableValue = _resolver.Value.ResolveProperty(variableName);

				// Check if the value of the variable is a number
				if (variableValue.Value.IntValue == null)
				{
					return new DeclarationResult()
					{
						IndexedVariable = variableValue,
						TypeName = variableValue.Value.TypeData.Name
					};
				}

				index = variableValue.Value.IntValue.ToString()!;
			}

			var name = context.ID().First().GetText();

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name), "Error: There was no name given for the array access.");
			}

			if (!ApplicationState.CurrentScope.LookupManager.IsVariable(name))
			{
				throw new VariableDoesNotExistException(nameof(name));
			}

			// Get the array from the lookup manager
			Variable array = _resolver.Value.ResolveProperty(name);

			// If the array has no value or is null then throw exception
			if (array == null || !array.HasValue())
			{
				throw new ArgumentNullException(nameof(array), "Error: The array has no value.");
			}

			// If the given variable name is not a collection then throw exception
			if (array.Value.TypeData.Name != "collection")
			{
				throw new InvalidTypeException(nameof(array));
			}

			return this.GetArrayValueAtIndex(index, array);
		}

		public override DeclarationResult VisitRange_expression([NotNull] Verse.Range_expressionContext context)
		{
			// Get the numbers from the range expression
			// Example: 1..10 -> Get 1 and 10
			List<int> rangeNumbers = new List<int>();
			List<int> result = new List<int>();
			int start;
			int step;
			int end;

			for (int i = 0; i < context.INT().Length; i++)
			{
				int num = int.Parse(context.INT(i).GetText());
				rangeNumbers.Add(num);
			}

			// Check if a step like 1,3..10 or just 1..10 is given.
			if (rangeNumbers.Count > 2)
			{
				start = rangeNumbers.First();
				step = rangeNumbers.ElementAt(1) - start;
				end = rangeNumbers.Last();
			}
			else
			{
				start = rangeNumbers.First();
				step = 1;
				end = rangeNumbers.Last();
			}

			// Add the numbers within the range to the list.
			for (int i = start; i <= end; i += step)
			{
				result.Add(i);
			}

			// Create anonym variables from the values of the result.
			List<Variable> anonymVariables = new List<Variable>();

			foreach (var value in result)
			{
				anonymVariables.Add(new Variable("undefined", new ValueObject("int", value)));
			}

			DeclarationResult declarationResult = new DeclarationResult();
			declarationResult.CollectionVariable = new VerseCollection(anonymVariables);
			declarationResult.CollectionVariable.Values = anonymVariables;

			return _typeInferencer.Value.InferGivenType(declarationResult);
		}

		private DeclarationResult GetArrayValueAtIndex(string index, Variable array)
		{
			// Get the list of variables in the array and parse the index string to a number
			var variables = array.Value.CollectionVariable.Values;
			int indexNumber = int.Parse(index);
			DeclarationResult declarationResult = new DeclarationResult();

			// Check if the index is valid
			// If not then return false? as value
			if (indexNumber < 0 || indexNumber >= variables.Count)
			{
				declarationResult.TypeName = "false?";
			}
			else
			{
				// Get the single variable from the list
				declarationResult.IndexedVariable = variables[indexNumber];
				declarationResult.TypeName = variables[indexNumber].Value.TypeData.Name;
			}

			// Get the value of the variable depending on its variable type
			return _typeInferencer.Value.InferGivenType(declarationResult);
		}
	}
}
