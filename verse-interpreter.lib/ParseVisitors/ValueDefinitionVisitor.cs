using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;
using System.Xml.Linq;

namespace verse_interpreter.lib.ParseVisitors
{
    public class ValueDefinitionVisitor : AbstractVerseVisitor<DeclarationResult>
    {
        private readonly TypeInferencer _typeInferencer;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly TypeConstructorVisitor _constructorVisitor;
        private readonly Lazy<TypeMemberVisitor> _memberVisitor;
        private readonly CollectionParser _collectionParser;
        private readonly GeneralEvaluator _evaluator;
        private readonly PropertyResolver _resolver;
        private readonly Lazy<FunctionCallVisitor> _functionCallVisitor;
        private readonly Lazy<DeclarationParser> _declarationParser;
        

        public ValueDefinitionVisitor(ApplicationState applicationState,
                                      TypeInferencer typeInferencer,
                                      Lazy<FunctionCallVisitor> functionVisitor,
                                      Lazy<DeclarationParser> declarationParser,
                                      ExpressionVisitor expressionVisitor,
                                      TypeConstructorVisitor constructorVisitor,
                                      Lazy<TypeMemberVisitor> memberVisitor,
                                      CollectionParser collectionParser,
                                      GeneralEvaluator evaluator,
                                      PropertyResolver resolver) : base(applicationState)
        {
            _typeInferencer = typeInferencer;
            _expressionVisitor = expressionVisitor;
            _constructorVisitor = constructorVisitor;
            _memberVisitor = memberVisitor;
            _collectionParser = collectionParser;
            _evaluator = evaluator;
            _resolver = resolver;
            _functionCallVisitor = functionVisitor;
            _declarationParser = declarationParser;
        }

        public override DeclarationResult VisitValue_definition([NotNull] Verse.Value_definitionContext context)
        {
            var maybeInt = context.INT();
            if(maybeInt != null)
            {
                return new DeclarationResult()
                {
                    Value = maybeInt.GetText(),
                    TypeName = "int"
                };
            }

            return HandleValueAssignment(context);
        }

        private DeclarationResult HandleValueAssignment([NotNull] Verse.Value_definitionContext context)
        {
            // Instead of a big if, lets use the visitor to determine which kind of value definition it actually is.
            var declarationResult = context.GetChild(0).Accept(this);
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
                DynamicType = typeInstance
            };

            return _typeInferencer.InferGivenType(declarationResult);
        }

        public override DeclarationResult VisitExpression(Verse.ExpressionContext context)
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
            _evaluator.ExecuteExpression(expression);
            return declarationResult;
        }

        public override DeclarationResult VisitFunction_call(Verse.Function_callContext context)
        {
            DeclarationResult declarationResult = new DeclarationResult();
            var functionCallResult = _functionCallVisitor.Value.Visit(context);

            var intValue = functionCallResult.ArithmeticExpression;
            var stringValue = functionCallResult.StringExpression;

            declarationResult.Value = intValue != null ? intValue.ResultValue.ToString() : stringValue != null ?
                stringValue.Value : throw new NotImplementedException();

            return _typeInferencer.InferGivenType(declarationResult);
        }

        public override DeclarationResult VisitType_member_access(Verse.Type_member_accessContext context)
        {
            DeclarationResult   declarationResult = new DeclarationResult();
            var result = _memberVisitor.Value.Visit(context);
            var variable = _resolver.ResolveProperty(result.AbsoluteCall);

            declarationResult.TypeName = variable.Value.TypeName;
            declarationResult.CollectionVariable = variable.Value.CollectionVariable;
            declarationResult.DynamicType = variable.Value.DynamicType;
            declarationResult.Value =
                variable!.Value.TypeName == "int" ? variable!.Value.IntValue.ToString() : variable!.Value.StringValue;

            return declarationResult;
        }

        public override DeclarationResult VisitArray_literal([NotNull] Verse.Array_literalContext context)
        {
            List<Variable> variables = new List<Variable>();
            var result = _collectionParser.GetParameters(context.array_elements());

            if (result.ValueElements != null)
            {
                foreach (var valueDef in result.ValueElements)
                {
                    var variableResult = VariableConverter.Convert(valueDef.Accept(this));
                    variables.Add(variableResult);
                }
            }

            if (result.DeclarationElements != null)
            {
                foreach (var declDef in result.DeclarationElements)
                {
                    var variableResult = _declarationParser.Value.ParseDeclaration(declDef);
                    variables.Add(VariableConverter.Convert(variableResult));
                }
            }

            if (result.VariableElements != null)
            {
                foreach (var variable in result.VariableElements)
                {
                    var variableResult = ApplicationState.CurrentScope.LookupManager.GetVariable(variable);
                    variables.Add(variableResult);
                }
            }

            DeclarationResult declarationResult = new DeclarationResult();
            declarationResult.TypeName = "collection";
            declarationResult.CollectionVariable = new VerseCollection(variables);

            return _typeInferencer.InferGivenType(declarationResult);
        }

        public override DeclarationResult VisitArray_index([NotNull] Verse.Array_indexContext context)
        {
            // Get the index and the name of the array variable
            var index = context.INT().GetText();
            var name = context.ID().First().GetText();

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name), "Error: There was no name given for the array access.");
            }

            if (index == null)
            {
                throw new ArgumentNullException(nameof(index), "Error: There was no index given for the array access.");
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
            if (array.Value.TypeName != "collection")
            {
                throw new InvalidTypeException(nameof(array));
            }

            return this.GetArrayValueAtIndex(index, array);
        }

        private DeclarationResult GetArrayValueAtIndex(string index, Variable array)
        {
            // Get the list of variables in the array and parse the index string to a number
            var variables = array.Value.CollectionVariable.Values;
            int indexNumber = int.Parse(index);

            // Check if the index is valid
            if (indexNumber < 0 || indexNumber >= variables.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(indexNumber), "Error: The given index value was invalid!");
            }

            // Get the single variable from the list
            DeclarationResult declarationResult = new DeclarationResult();
            declarationResult.IndexedVariable = variables[indexNumber];
            declarationResult.TypeName = variables[indexNumber].Value.TypeName;

            // Get the value of the variable depending on its variable type
            return _typeInferencer.InferGivenType(declarationResult);
        }
    }
}
