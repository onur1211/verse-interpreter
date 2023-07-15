using System.Runtime.InteropServices;
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
            if(declarationResult.TypeName != "undefined")
            {
                return declarationResult;
            }

            IsNumber(declarationResult);
            IsCustomType(declarationResult);
            IsCollection(declarationResult);


            return declarationResult;
        }

        private DeclarationResult IsNumber(DeclarationResult declarationResult)
        {
            if (declarationResult.Value == null)
            {
                return declarationResult;
            }

            if (!int.TryParse(declarationResult.Value, out _))
            {
                declarationResult.TypeName = "string";
                return declarationResult;
            }

			declarationResult.TypeName = "int";
            return declarationResult;
		}

		private DeclarationResult IsCustomType(DeclarationResult declarationResult)
        {
            if (declarationResult.CustomType == null)
            {
                return declarationResult;
            }
            declarationResult.TypeName = declarationResult.CustomType.Name;

			if (!_state.Types.ContainsKey(declarationResult.TypeName) && !_state.WellKnownTypes.Any(x => x.Name == declarationResult.TypeName))
			{
				throw new UnknownTypeException(declarationResult.TypeName);
			}

            return declarationResult;
		}

        private DeclarationResult IsCollection(DeclarationResult declarationResult)
        {
            if (declarationResult.CollectionVariable == null)
            {
                return declarationResult;
            }

            return declarationResult;
        }
    }
}
