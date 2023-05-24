using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Validators;
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

                if (declarationResult.CollectionVariable != null) 
                {
                    declarationResult.TypeName = "collection";
                }
            }

            if (!_state.Types.ContainsKey(declarationResult.TypeName) && !_state.WellKnownTypes.Contains(declarationResult.TypeName))
            {
                throw new UnknownTypeException(declarationResult.TypeName);
            }

            if (declarationResult.ExpressionResults != null)
            {
                declarationResult.TypeName = HandleExpressions(declarationResult);
            }

            return declarationResult;
        }

        private string HandleExpressions(DeclarationResult declarationResult)
        {
            return _expressionValidator.GetExpressionType(declarationResult.ExpressionResults);
        }
    }
}
