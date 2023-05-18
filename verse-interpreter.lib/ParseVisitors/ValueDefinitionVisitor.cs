using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class ValueDefinitionVisitor : AbstractVerseVisitor<DeclarationResult>
    {
        private readonly TypeInferencer _typeInferencer;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly TypeConstructorVisitor _constructorVisitor;
        private readonly EvaluatorWrapper _baseEvaluator;

        public ValueDefinitionVisitor(ApplicationState applicationState,
                                      TypeInferencer typeInferencer,
                                      ExpressionVisitor expressionVisitor,
                                      TypeConstructorVisitor constructorVisitor,
                                      EvaluatorWrapper evaluator) : base(applicationState)
        {
            _typeInferencer = typeInferencer;
            _expressionVisitor = expressionVisitor;
            _constructorVisitor = constructorVisitor;
            _baseEvaluator = evaluator;
        }

        public override DeclarationResult VisitValue_definition([NotNull] Verse.Value_definitionContext context)
        {
            DeclarationResult declarationResult = new DeclarationResult();

            var maybeInt = context.INT();
            var maybeArrayLiteral = context.array_literal();
            var maybeExpression = context.expression();
            var maybeString = context.string_rule();
            var maybeConstructor = context.constructor_body();

            if (maybeConstructor != null)
            {
                var typeInstance = maybeConstructor.Accept(_constructorVisitor);
                declarationResult.TypeName = "dynamic";
                declarationResult.DynamicType = typeInstance;
            }

            if (maybeInt != null)
            {
                declarationResult.Value = maybeInt.GetText();
            }

            if (maybeArrayLiteral != null)
            {
                declarationResult = maybeArrayLiteral.Accept(this);
            }

            if (maybeExpression != null)
            {
                var expression = _expressionVisitor.Visit(maybeExpression);
                _expressionVisitor.Clean();
                var value = _baseEvaluator.ArithmeticEvaluator.Evaluate(expression).ResultValue.ToString();
                declarationResult.Value = value == null ? "false?" : value;
            }

            if (maybeString != null)
            {
                declarationResult.Value = maybeString.SEARCH_TYPE().GetText().Replace("\"", "");
            }

            return _typeInferencer.InferGivenType(declarationResult);
        }

        public override DeclarationResult VisitArray_literal([NotNull] Verse.Array_literalContext context)
        {
            List<DeclarationResult> declarationResult = new List<DeclarationResult>();

            // Check the children of this context recursive.
            // For variable declarations like y:int
            var valueDefinitionContext = context.array_elements();

            if (valueDefinitionContext.value_definition() != null)
            {
                var result = this.Visit(valueDefinitionContext.value_definition());

                //foreach (var valueDef in valueDefinitionContext.value_definition())
                //{
                //    declarationResult.Add(valueDef.Accept(this));
                //}

                List<Variable> variables = new List<Variable>();
                string name = string.Empty;

                foreach (var decResult in declarationResult)
                {
                    name = decResult.Name;
                    variables.Add(VariableConverter.Convert(decResult));
                }

                CollectionVariable collectionVariable = new CollectionVariable(name, "collection", variables.ToArray());

                DeclarationResult finalResult = new DeclarationResult();
                finalResult.Name = name;
                finalResult.CollectionVariable = collectionVariable;

                finalResult = _typeInferencer.InferGivenType(finalResult);

                return finalResult;
            }

            if (valueDefinitionContext.declaration() != null)
            {

            }

            throw new NotImplementedException();
        }
    }
}
