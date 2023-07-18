using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;
using verse_interpreter.lib.Parser.ValueDefinitionParser;

namespace verse_interpreter.lib.ParseVisitors
{
	public class ValueDefinitionVisitor : AbstractVerseVisitor<DeclarationResult?>
    {
        private readonly TypeInferencer _typeInferencer;
        private readonly GeneralEvaluator _evaluator;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly TypeConstructorVisitor _constructorVisitor;
        private readonly Lazy<TypeMemberVisitor> _memberVisitor;
        private readonly CollectionParser _collectionParser;
		private readonly ExpressionValueParser _expressionValueParser;
        private readonly PropertyResolver _resolver;
        private readonly Lazy<FunctionCallVisitor> _functionCallVisitor;
        private readonly Lazy<DeclarationParser> _declarationParser;
        

        public ValueDefinitionVisitor(ApplicationState applicationState,
                                      GeneralEvaluator evaluator,
                                      TypeInferencer typeInferencer,
                                      Lazy<FunctionCallVisitor> functionVisitor,
                                      Lazy<DeclarationParser> declarationParser,
                                      ExpressionVisitor expressionVisitor,
                                      TypeConstructorVisitor constructorVisitor,
                                      Lazy<TypeMemberVisitor> memberVisitor,
                                      CollectionParser collectionParser,
                                      ExpressionValueParser expressionValueParser,
                                      PropertyResolver resolver) : base(applicationState)
        {
            _typeInferencer = typeInferencer;
            _evaluator = evaluator;
            _expressionVisitor = expressionVisitor;
            _constructorVisitor = constructorVisitor;
            _memberVisitor = memberVisitor;
            _collectionParser = collectionParser;
			_expressionValueParser = expressionValueParser;
            _resolver = resolver;
            _functionCallVisitor = functionVisitor;
            _declarationParser = declarationParser;
        }

        public override DeclarationResult VisitValue_definition([NotNull] Verse.Value_definitionContext context)
        {
            var maybeInt = context.INT();
            var maybeNoValue = context.NOVALUE();
            var maybeID = context.ID();

            if (maybeNoValue != null)
            {
                return new DeclarationResult()
                {
                    TypeName = "false?"
                };
            }

            // Check if the value is a number
            if (maybeInt != null)
            {
                return new DeclarationResult()
                {
                    Value = maybeInt.GetText(),
                    TypeName = "int"
                };
            }

            // Check if the value is a ID which is a variable name
            if (maybeID != null)
            {
                // Get the variable name
                var variableName = maybeID.GetText();

                // Get the actual variable intance from the lookup table
                Variable variable = _resolver.ResolveProperty(variableName);

                // Convert the variable back to a declaration result
                return VariableConverter.ConvertBack(variable);
            }

            return HandleValueAssignment(context);
        }

        private DeclarationResult HandleValueAssignment([NotNull] Verse.Value_definitionContext context)
        {
            // Instead of a big if, lets use the visitor to determine which kind of value definition it actually is.
            var declarationResult = context.children.First().Accept(this);

            if (declarationResult == null)
            {
                throw new NotImplementedException();
            }

            return declarationResult;
        }

        public override DeclarationResult VisitString_rule(Verse.String_ruleContext context)
        {
            DeclarationResult declarationResult = new DeclarationResult
            {
                Value = context.SEARCH_TYPE().GetText().Replace("\"", ""),
                TypeName = "string"
            };

            return _typeInferencer.InferGivenType(declarationResult);
        }

        public override DeclarationResult VisitConstructor_body(Verse.Constructor_bodyContext context)
        {
            var typeInstance = context.Accept(_constructorVisitor);

            DeclarationResult declarationResult = new DeclarationResult
            {
                TypeName = typeInstance.Name,
                CustomType = typeInstance
            };

            return _typeInferencer.InferGivenType(declarationResult);
        }

        public override DeclarationResult? VisitExpression(Verse.ExpressionContext context)
        {
            var expression = _expressionVisitor.Visit(context);

            _expressionVisitor.Clean();
            DeclarationResult declarationResult = new DeclarationResult
            {
                ExpressionResults = expression
            };
            _evaluator.ArithmeticExpressionResolved += (x,y) =>
            {
                declarationResult.Value = y.Result.ResultValue.ToString()!;
                declarationResult.ExpressionResults = null;
                declarationResult.TypeName = "int";
            };
            _evaluator.StringExpressionResolved += (x, y) =>
            {
                declarationResult.ExpressionResults = null;
                declarationResult.Value = y.Result.Value;
                declarationResult.TypeName = "string";
            };
            _evaluator.ExpressionWithNoValueFound += (x, y) =>
            {
                declarationResult.ExpressionResults = null;
                declarationResult.TypeName = "false?";
            };

            _evaluator.ExecuteExpression(expression);
            return declarationResult;
        }

        public override DeclarationResult VisitFunction_call(Verse.Function_callContext context)
        {
            var functionCallResult = _functionCallVisitor.Value.Visit(context);
            
            return _typeInferencer.InferGivenType(DeclarationResultConverter.ConvertFunctionResult(functionCallResult));
        }

        public override DeclarationResult VisitType_member_access(Verse.Type_member_accessContext context)
        {
            DeclarationResult declarationResult = new DeclarationResult();
            var result = _memberVisitor.Value.Visit(context);
            var variable = _resolver.ResolveProperty(result.AbsoluteCall);

            declarationResult.TypeName = variable.Value.TypeData.Name;
            declarationResult.CollectionVariable = variable.Value.CollectionVariable;
            declarationResult.CustomType = variable.Value.CustomType;
            declarationResult.Value =
                variable!.Value.TypeData.Name == "int" ? variable!.Value.IntValue.ToString() : variable!.Value.StringValue;

            return declarationResult;
        }

        public override DeclarationResult VisitArray_literal([NotNull] Verse.Array_literalContext context)
        {
            List<Variable> variables = new List<Variable>();
            DeclarationResult rangeExpressionResult = new DeclarationResult();
            var result = _collectionParser.GetParameters(context.array_elements());
            // Check if there are value elements in the collection
            // Example: myArray:=(1,2,3) => 1,2 and 3 are value elements
            if (result.ValueElements != null)
            {
                foreach (var valueDef in result.ValueElements)
                {
                    // Accept the range expression
                    rangeExpressionResult = valueDef.Accept(this);

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

            return _typeInferencer.InferGivenType(declarationResult);
        }

        public override DeclarationResult VisitArray_index([NotNull] Verse.Array_indexContext context)
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
                Variable variableValue = _resolver.ResolveProperty(variableName);

                // Check if the value of the variable is a number
                if (variableValue.Value.IntValue == null)
                {
                    throw new InvalidTypeException(nameof(variableValue));
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
            Variable array = _resolver.ResolveProperty(name);

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

            return _typeInferencer.InferGivenType(declarationResult);
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
            return _typeInferencer.InferGivenType(declarationResult);
        }
    }
}
