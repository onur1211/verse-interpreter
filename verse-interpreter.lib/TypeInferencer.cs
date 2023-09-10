using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects.Validators;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib
{
	public class TypeInferencer
	{
		private ApplicationState _state;
		private readonly ExpressionValidator _expressionValidator;

		public TypeInferencer(ApplicationState applicationState,
							  ExpressionValidator expressionValidator)
		{
			_state = applicationState;
			_expressionValidator = expressionValidator;
		}

		/// <summary>
		/// Tries to infer the type by checking if the value is castable to an integer and if the given type is actually known in the current application state.
		/// </summary>
		/// <param name="declarationResult"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public DeclarationResult InferGivenType(DeclarationResult declarationResult)
		{
			if (declarationResult == null)
			{
				throw new ArgumentNullException("The specified input object is null!");
			}

			if (declarationResult.TypeName != "undefined")
			{
				return declarationResult;
			}

			IsNumber(declarationResult);
			IsCustomType(declarationResult);
			IsCollection(declarationResult);
			IsChoice(declarationResult);

			return declarationResult;
		}

		/// <summary>
		/// Checks if the value of the DeclarationResult object is castable to an integer. If not, it updates the TypeName property to "string".
		/// </summary>
		/// <param name="declarationResult"></param>
		/// <returns></returns>
		private DeclarationResult IsNumber(DeclarationResult declarationResult)
		{
			if (declarationResult.LiteralValue == null)
			{
				return declarationResult;
			}

			if (!int.TryParse(declarationResult.LiteralValue, out _))
			{
				declarationResult.TypeName = "string";
				return declarationResult;
			}

			declarationResult.TypeName = "int";
			return declarationResult;
		}

		/// <summary>
		/// Checks if the DeclarationResult object has a custom type assigned. It updates the TypeName property to the name of the custom type and verifies if the type is known in the application state. If the type is not known, it throws an UnknownTypeException.
		/// </summary>
		/// <param name="declarationResult"></param>
		/// <returns></returns>
		/// <exception cref="UnknownTypeException"></exception>
		private DeclarationResult IsCustomType(DeclarationResult declarationResult)
		{
			if (!declarationResult.CustomType.HasValue)
			{
				return declarationResult;
			}

			declarationResult.TypeName = declarationResult.CustomType.Value.Name;

			if (!_state.Types.ContainsKey(declarationResult.TypeName) && !_state.WellKnownTypes.Any(x => x.Name == declarationResult.TypeName))
			{
				throw new UnknownTypeException(declarationResult.TypeName);
			}

			return declarationResult;
		}

		/// <summary>
		/// Checks if the DeclarationResult object represents a collection variable. If so, it updates the TypeName property to "collection".
		/// </summary>
		/// <param name="declarationResult"></param>
		/// <returns></returns>
		private DeclarationResult IsCollection(DeclarationResult declarationResult)
		{
			if (declarationResult.CollectionVariable == null)
			{
				return declarationResult;
			}
			else
			{
				declarationResult.TypeName = "collection";
			}

			return declarationResult;
		}

		private DeclarationResult IsChoice(DeclarationResult declarationResult)
		{
			if (declarationResult.ChoiceResult == null)
			{
				return declarationResult;
			}
			string typeName = null!;
			var current = declarationResult.ChoiceResult;

			do
			{

				var type = declarationResult.ChoiceResult.Literals.FirstOrDefault()?.Value.TypeData.Name;
				typeName ??= type;

				if (typeName != type)
				{
					throw new InvalidTypeCombinationException(typeName);
				}

				declarationResult.TypeName = typeName;
				current = current.Next;
				if (current == null)
				{
					break;
				}

			} while (current.Next != null);

			return declarationResult;
		}
	}
}
