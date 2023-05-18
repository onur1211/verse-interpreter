using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Factories;

namespace verse_interpreter.lib
{
    public class ExpressionValidator
    {
        private readonly ApplicationState _applicationState;

        public ExpressionValidator(ApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        /// <summary>
        /// Checks whether or not all expression elements are of the same type.
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public bool IsTypeConformityGiven(List<List<ExpressionResult>> expressions)
        {
            string typeName = string.Empty;
            foreach (var expression in expressions)
            {
                foreach (var exp in expression)
                {
                    if (!string.IsNullOrEmpty(exp.ValueIdentifier) && typeName == string.Empty)
                    {
                        typeName = _applicationState.CurrentScope.LookupManager.GetVariable(exp.ValueIdentifier).Type;
                    }
                    if (!string.IsNullOrEmpty(exp.ValueIdentifier) && _applicationState.CurrentScope.LookupManager.GetVariable(exp.ValueIdentifier).Type != typeName)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public string GetExpressionType(List<List<ExpressionResult>> expressions)
        {
            if (!IsTypeConformityGiven(expressions))
            {
                throw new InvalidTypeCombinationException("The specified type contains multiple differently typed values");
            }

            foreach (var expression in expressions)
            {
                foreach (var exp in expression)
                {
                    if (!string.IsNullOrEmpty(exp.ValueIdentifier))
                    {
                        return _applicationState.CurrentScope.LookupManager.GetVariable(exp.ValueIdentifier).Type;
                    }
                }
            }

            throw new InvalidTypeCombinationException("The specified type contains multiple differently typed values");
        }
    }
}
