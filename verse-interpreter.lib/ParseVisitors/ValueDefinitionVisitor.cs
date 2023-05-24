﻿using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.DataVisitors;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.lib.Visitors
{
    public class ValueDefinitionVisitor : AbstractVerseVisitor<DeclarationResult>
    {
        private readonly TypeInferencer _typeInferencer;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly TypeConstructorVisitor _constructorVisitor;
        private readonly CollectionParser _collectionParser;

        public event EventHandler<DeclarationInArrayFoundEventArgs> DeclarationInArrayFound;

        public ValueDefinitionVisitor(ApplicationState applicationState,
                                      TypeInferencer typeInferencer,
                                      ExpressionVisitor expressionVisitor,
                                      TypeConstructorVisitor constructorVisitor,
                                      CollectionParser collectionParser) : base(applicationState)
        {
            _typeInferencer = typeInferencer;
            _expressionVisitor = expressionVisitor;
            _constructorVisitor = constructorVisitor;
            _collectionParser = collectionParser;
        }

        public override DeclarationResult VisitValue_definition([NotNull] Verse.Value_definitionContext context)
        {
            DeclarationResult declarationResult = new DeclarationResult();

            var maybeInt = context.INT();
            var maybeArrayLiteral = context.array_literal();
            var maybeArrayIndex = context.array_index();
            var maybeExpression = context.expression();
            var maybeString = context.string_rule();
            var maybeConstructor = context.constructor_body();

            if (maybeConstructor != null)
            {
                var typeInstance = maybeConstructor.Accept(_constructorVisitor);
                declarationResult.TypeName = typeInstance.Name;
                declarationResult.DynamicType = typeInstance;
            }

            if (maybeInt != null)
            {
                declarationResult.Value = maybeInt.GetText();
            }

            if (maybeArrayLiteral != null)
            {
                declarationResult = this.VisitArray_literal(maybeArrayLiteral);
            }

            if (maybeArrayIndex != null)
            {
                declarationResult = this.VisitArray_index(maybeArrayIndex);
            }

            if (maybeExpression != null)
            {
                var expression = _expressionVisitor.Visit(maybeExpression);
                _expressionVisitor.Clean();

                declarationResult.ExpressionResults = expression;
            }

            if (maybeString != null)
            {
                declarationResult.Value = maybeString.SEARCH_TYPE().GetText().Replace("\"", "");
            }

            return _typeInferencer.InferGivenType(declarationResult);
        }

        public override DeclarationResult VisitArray_literal([NotNull] Verse.Array_literalContext context)
        {
            List<Variable> variables = new List<Variable>();
            var result = _collectionParser.GetParameters(context.array_elements());

            if (result.ValueElements != null)
            {
                foreach (var valueDef in result.ValueElements)
                {
                    var variableResult = VariableConverter.Convert(valueDef.Accept(this), this.ApplicationState);
                    variables.Add(variableResult);
                }
            }

            if (result.DeclarationElements != null)
            {
                foreach (var declDef in result.DeclarationElements)
                {
                    this.FireDeclarationInArrayFoundEvent(this, declDef);
                }
            }

            DeclarationResult declarationResult = new DeclarationResult();
            declarationResult.TypeName = "collection";
            declarationResult.CollectionVariable = new CollectionVariable(declarationResult.Name, "collection", variables);

            return declarationResult;
        }

        public override DeclarationResult VisitArray_index([NotNull] Verse.Array_indexContext context)
        {
            // Get the index and the name of the array variable
            var index = context.INT().GetText();
            var name = context.ID().GetText();

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
            Variable array = ApplicationState.CurrentScope.LookupManager.GetVariable(name);

            // If the array has no value or is null then throw exception
            if (array == null || !array.HasValue()) 
            {
                throw new ArgumentNullException(nameof(array), "Error: The array has no value.");
            }

            // If the given variable name is not a collection then throw exception
            if (array.Type != "collection")
            {
                throw new InvalidTypeException(nameof(array));
            }

            return this.GetArrayValueAtIndex(index, array);
        }

        private DeclarationResult GetArrayValueAtIndex(string index, Variable array)
        {
            // Get the list of variables in the array and parse the index string to a number
            var variables = array.AcceptCollection(new VariableVisitor());
            int indexNumber = int.Parse(index);

            // Check if the index is valid
            if (indexNumber < 0 || indexNumber >= variables.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(indexNumber), "Error: The given index value was invalid!");
            }

            // Get the single variable from the list
            var variable = variables[indexNumber];
            string value = string.Empty;
            string typeName = variable.Type;
            List<Variable> collectionValues = new List<Variable>();
            DeclarationResult declarationResult = new DeclarationResult();

            // Get the value of the variable depending on its variable type
            switch (variable.Type)
            {
                case "int":
                    value = variable.AcceptInt(new VariableVisitor()).ToString()!;
                    declarationResult.Value = value;
                    declarationResult.TypeName = typeName;
                    break;

                case "string":
                    value = variable.AcceptString(new VariableVisitor()).ToString()!;
                    declarationResult.Value = value;
                    declarationResult.TypeName = typeName;
                    break;

                case "collection":
                    collectionValues = variable.AcceptCollection(new VariableVisitor());
                    declarationResult.CollectionVariable = new CollectionVariable("undefined", typeName, collectionValues);
                    declarationResult.TypeName = typeName;
                    break;

                default:
                    value = variable.AcceptDynamicType(new VariableVisitor()).ToString()!;
                    declarationResult.Value = value;
                    declarationResult.TypeName = typeName;
                    break;
            }

            return declarationResult;
        }

        protected virtual void FireDeclarationInArrayFoundEvent(object sender, Verse.DeclarationContext declarationContext)
        {
            if (this.DeclarationInArrayFound != null)
            {
                this.DeclarationInArrayFound(sender, new DeclarationInArrayFoundEventArgs(declarationContext));
            }
        }
    }
}
