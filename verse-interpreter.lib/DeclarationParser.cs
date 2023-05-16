using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.lib
{
    public class DeclarationParser
    {
        private ApplicationState _state;
        private TypeInferencer _inferencer;
        private ExpressionVisitor _expressionVisitor;
        private readonly IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>> _arithmeticEvaluator;
        private readonly IEvaluator<string, List<List<ExpressionResult>>> _stringEvaluator;

        public DeclarationParser(ApplicationState applicationState,
                                 TypeInferencer typeInferencer,
                                 ExpressionVisitor expressionVisitor,
                                 IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>> arithmeticEvaluator,
                                 IEvaluator<string, List<List<ExpressionResult>>> stringEvaluator)
        {
            _state = applicationState;
            _inferencer = typeInferencer;
            _expressionVisitor = expressionVisitor;
            _arithmeticEvaluator = arithmeticEvaluator;
            _stringEvaluator = stringEvaluator;
        }

        public DeclarationResult ParseDeclaration(Verse.DeclarationContext context)
        {
            var assignmentOperatorKind = context.children[1].GetText();
            return assignmentOperatorKind switch
            {
                ":" => ParseBringToScopeOperator(context),
                "=" => ParseAssignValueToExistingVariable(context),
                ":=" => ParseGiveValueOperator(context),
                _ => throw new NotImplementedException(),
            };
        }

        private DeclarationResult ParseBringToScopeOperator(Verse.DeclarationContext context)
        {
            string name = context.ID().GetText();
            string type = context.type().GetText();

            return new DeclarationResult()
            {
                Name = name,
                TypeName = type,
            };
        }

        private DeclarationResult ParseGiveValueOperator(Verse.DeclarationContext context)
        {
            var variable = ParseValueAssignment(context);
            return _inferencer.InferGivenType(variable);
        }

        private DeclarationResult ParseAssignValueToExistingVariable(Verse.DeclarationContext context)
        {
            var variableName = context.ID().GetText();
            if (!_state.CurrentScope.LookupManager.IsVariable(variableName))
            {
                throw new InvalidOperationException($"Invalid usage of out of scope variable {nameof(variableName)}");
            }

            return ParseValueAssignment(context);
        }

        private DeclarationResult ParseValueAssignment(Verse.DeclarationContext context)
        {
            DeclarationResult declarationResult = new DeclarationResult();
            declarationResult.Name = context.ID().GetText();

            var maybeConstructor = context.constructor_body();
            var maybeInt = context.INT();
            var maybeExpression = context.expression();
            var maybeString = context.string_rule();

            if (maybeConstructor != null)
            {

            }
            if (maybeInt != null)
            {
                declarationResult.Value = maybeInt.GetText();
            }
            if (maybeExpression != null)
            {
                var expression = _expressionVisitor.Visit(maybeExpression);
                _expressionVisitor.Clean();
                var value = _arithmeticEvaluator.Evaluate(expression).ResultValue.ToString();
                declarationResult.Value = value== null? "false?" : value;
            }
            if (maybeString != null)
            {
                declarationResult.Value = maybeString.SEARCH_TYPE().GetText().Replace("\"", "");
            }

            return declarationResult;
        }
    }
}
