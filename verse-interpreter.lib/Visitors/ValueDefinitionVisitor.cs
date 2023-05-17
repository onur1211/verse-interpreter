using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class ValueDefinitionVisitor : AbstractVerseVisitor<DeclarationResult>
    {
        private readonly ApplicationState _applicationState;
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
            _applicationState = applicationState;
            _typeInferencer = typeInferencer;
            _expressionVisitor = expressionVisitor;
            _constructorVisitor = constructorVisitor;
            _baseEvaluator = evaluator;
        }

        public override DeclarationResult VisitValue_definition([NotNull] Verse.Value_definitionContext context)
        {
            DeclarationResult declarationResult = new DeclarationResult();

            var maybeInt = context.INT();
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
    }
}
