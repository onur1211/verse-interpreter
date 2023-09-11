using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Exceptions;

namespace verse_interpreter.lib.Data.ResultObjects.Validators
{
    public class ExpressionValidator : IValidator<List<List<ExpressionResult>>>
    {
        private readonly ApplicationState _applicationState;
        private readonly PropertyResolver _resolver;

        public ExpressionValidator(ApplicationState applicationState, PropertyResolver resolver)
        {
            _applicationState = applicationState;
            _resolver = resolver;
        }

        /// <summary>
        /// Checks whether or not all expression elements are of the same type.
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public bool IsTypeConformityGiven(List<List<ExpressionResult>> expressions)
        {
            string typeName = null!;

            foreach (var expression in expressions)
            {
                string currentIterationType = null!;

                foreach (var exp in expression)
                {
                    if (currentIterationType == "false?")
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(exp.StringValue))
                    {
                        currentIterationType = "string";
                        typeName ??= currentIterationType;
                    }

                    if (exp.IntegerValue != null)
                    {
                        currentIterationType = "int";
                        typeName ??= currentIterationType;
                    }

                    if (!string.IsNullOrEmpty(exp.ValueIdentifier))
                    {
                        currentIterationType = _resolver.ResolveProperty(exp.ValueIdentifier).Value.TypeData.Name;
                        if (currentIterationType == "false?")
                        {
                            continue;
                        }
                        typeName ??= currentIterationType;
                    }

                    if (currentIterationType != typeName)
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
                throw new InvalidTypeCombinationException(
                    "The specified type contains multiple differently typed values");
            }

            var result = expressions.Last().Where(x => string.IsNullOrEmpty(x.Operator)).First();

            if (expressions.Select(x => x.Select(y => y.Operator)).Any(x => x.Any(y => y == ">" || y == "=" || y == "<")))
            {
                return "comparison";
            }
            if (!string.IsNullOrEmpty(result.ValueIdentifier))
            {
                return _resolver.ResolveProperty(result.ValueIdentifier).Value.TypeData.Name;
            }

            return result.TypeName;
        }
    }
}
