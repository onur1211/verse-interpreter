using Antlr4.Runtime.Misc;
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

            for (int i = 0; i < result.TotalElements; i++)
            {
                // Check if there is a value element in the collection with the current index i
                // Example: myArray:=(1,2,3) => 1,2 and 3 are value elements
                if (result.ValueElements.Where(x => x.Key == i).Count() == 1)
                {
                    var valueDef = result.ValueElements.Where(x => x.Key == i).First();
                    var valueDefResult = valueDef.Value.Accept(_valueDefinitionVisitor.Value)!;

                    var variableResult = VariableConverter.Convert(valueDefResult);
                    variables.Add(variableResult);
                }

                // Check if there is a declaration element in the collection with the current index i
                // Example: myArray:=(x:=1,2) => x:=1 is a declaration element
                if (result.DeclarationElements.Where(x => x.Key == i).Count() == 1)
                {
                    var declDef = result.DeclarationElements.Where(x => x.Key == i).First();
                    var variableResult = _declarationParser.Value.ParseDeclaration(declDef.Value);
                    variables.Add(VariableConverter.Convert(variableResult));
                }

                // Check if there is a variable element in the collection with the current index i
                // Example: x:=1; y:=2; myArray:=(x,y) => x and y are variable elements
                if (result.VariableElements.Where(x => x.Key == i).Count() == 1)
                {
                    var variable = result.VariableElements.Where(x => x.Key == i).First();
                    var variableResult = ApplicationState.CurrentScope.LookupManager.GetVariable(variable.Value);
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

        public override DeclarationResult VisitNumericArrayIndex([NotNull] Verse.NumericArrayIndexContext context)
        {
            // Example: myArray[0]
            string index = context.INT().GetText();
            var arrayName = context.ID().GetText();

            if (String.IsNullOrEmpty(index))
            {
                throw new ArgumentNullException(nameof(index), "Error: There was no index given for the array access.");
            }

            if (String.IsNullOrEmpty(arrayName))
            {
                throw new ArgumentNullException(nameof(arrayName), "Error: There was no name given for the array access.");
            }

            if (!ApplicationState.CurrentScope.LookupManager.IsVariable(arrayName))
            {
                throw new VariableDoesNotExistException(nameof(arrayName));
            }

            // Get the array from the lookup manager
            Variable array = _resolver.Value.ResolveProperty(arrayName);

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

        public override DeclarationResult VisitVariableNameArrayIndex([NotNull] Verse.VariableNameArrayIndexContext context)
        {
            // Example: myArray[x]
            var index = context.ID().ElementAt(1).GetText();
            var arrayName = context.ID().First().GetText();

            // Check if the given array name is not null or empty.
            if (String.IsNullOrEmpty(arrayName))
            {
                throw new ArgumentNullException(nameof(arrayName), "Error: There was no name given for the array access.");
            }

            // Check the given index for null or empty.
            if (String.IsNullOrEmpty(index))
            {
                throw new ArgumentNullException(nameof(index), "Error: There was no variable as index given for the array access.");
            }

            // Check if the given index is an actual variable.
            // No exception needs to be thrown because an invalid access simply results in false?
            if (!ApplicationState.CurrentScope.LookupManager.IsVariable(index))
            {
                return new DeclarationResult()
                {
                    TypeName = "false?"
                };
            }

            // Check if the given array name is an actual array.
            // No exception needs to be thrown because an invalid access simply results in false?
            if (!ApplicationState.CurrentScope.LookupManager.IsVariable(arrayName))
            {
                return new DeclarationResult()
                {
                    TypeName = "false?"
                };
            }

            // Get the actual variables from index and array name
            Variable variableValue = _resolver.Value.ResolveProperty(index);
            Variable array = _resolver.Value.ResolveProperty(arrayName);

            // If the given variable name is not a collection then throw exception
            if (array.Value.TypeData.Name != "collection")
            {
                throw new InvalidTypeException(nameof(array));
            }

            // If the array has no value or is null then throw exception
            if (array == null || !array.HasValue())
            {
                throw new ArgumentNullException(nameof(array), "Error: The array has no value.");
            }

            // Check if the value of the index variable is a number
            if (variableValue.Value.IntValue == null)
            {
                throw new InvalidTypeException(nameof(variableValue));
            }

            index = variableValue.Value.IntValue.ToString()!;

            return this.GetArrayValueAtIndex(index, array);
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
