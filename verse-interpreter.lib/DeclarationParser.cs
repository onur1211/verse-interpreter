using System.Net.Http.Headers;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
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
        private readonly ValueDefinitionVisitor _valueDefinitionVisitor;
        private readonly BackpropagationEventSystem _backPropagator;
        private readonly EvaluatorWrapper _evaluator;
        private readonly ExpressionValidator _validator;

        public DeclarationParser(ApplicationState applicationState,
                                 TypeInferencer typeInferencer,
                                 ValueDefinitionVisitor valueDefinitionVisitor,
                                 BackpropagationEventSystem backPropagator,
                                 EvaluatorWrapper evaluator,
                                 ExpressionValidator validator)
        {
            _state = applicationState;
            _inferencer = typeInferencer;
            _valueDefinitionVisitor = valueDefinitionVisitor;
            _valueDefinitionVisitor.DeclarationInArrayFound += _valueDefinitionVisitor_DeclarationInArrayFound;
            _backPropagator= backPropagator;
            _evaluator = evaluator;
            _validator = validator;
        }

        private void _valueDefinitionVisitor_DeclarationInArrayFound(object? sender, EventArguments.DeclarationInArrayFoundEventArgs e)
        {
            var result = this.ParseDeclaration(e.declarationContext);

            if (result != null) 
            {
                throw new NotImplementedException();
            }
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

            if (!(_state.Types.ContainsKey(type) || _state.WellKnownTypes.Contains(type)))
            {
                throw new InvalidOperationException($"The specified type \"{type}\" does not exist!");
            }

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

        /// <summary>
        /// Calls a <paramref type="ValueDefinitionVisitor"/> which fetches the values out of the parse tree.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private DeclarationResult ParseValueAssignment(Verse.DeclarationContext context)
        {
            DeclarationResult declarationResult = _valueDefinitionVisitor.Visit(context);
            declarationResult.Name = context.ID().GetText();
            
            if (declarationResult.CollectionVariable != null) 
            {
                declarationResult.CollectionVariable.Name = declarationResult.Name;
            }

            declarationResult = HandleExpressionAsValue(declarationResult);

            return declarationResult;
        }

        private DeclarationResult HandleExpressionAsValue(DeclarationResult declarationResult)
        {
            if (declarationResult.ExpressionResults == null)
            {
                return declarationResult;
            }
            var expressionType = _validator.GetExpressionType(declarationResult.ExpressionResults);
            ArithmeticExpression expression = new ArithmeticExpression();
            switch (expressionType)
            {
                case "string":
                      _evaluator.StringEvaluator.Evaluate(declarationResult.ExpressionResults);
                    throw new NotImplementedException();
                case "int":
                    expression = _evaluator.ArithmeticEvaluator.Evaluate(declarationResult.ExpressionResults);
                    break;
            }

            if(expression.PostponedExpression != null)
            {
                _backPropagator.AddExpression(declarationResult.Name, expression);
            }
            else
            {
                declarationResult.Value = expression.ResultValue.ToString();
            }

            if (declarationResult.CollectionVariable != null) 
            {
                declarationResult.CollectionVariable.Name = declarationResult.Name;
            }

            return declarationResult;
        }
    }
}
