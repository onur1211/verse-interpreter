using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Validators;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.ParseVisitors;

namespace verse_interpreter.lib.Parser
{
    public class DeclarationParser
    {
        private readonly ApplicationState _state;
        private readonly TypeInferencer _inferencer;
        private readonly ValueDefinitionVisitor _valueDefinitionVisitor;
        private readonly GeneralEvaluator _generalEvaluator;

        public DeclarationParser(ApplicationState applicationState,
                                 TypeInferencer typeInferencer,
                                 ValueDefinitionVisitor valueDefinitionVisitor,
                                 BackpropagationEventSystem backPropagator,
                                 ExpressionValidator validator,
                                  GeneralEvaluator generalEvaluator)
        {
            _state = applicationState;
            _inferencer = typeInferencer;
            _valueDefinitionVisitor = valueDefinitionVisitor;
            _valueDefinitionVisitor.DeclarationInArrayFound += _valueDefinitionVisitor_DeclarationInArrayFound;
            _generalEvaluator = generalEvaluator;
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

            declarationResult = HandleExpressionAsValue(declarationResult);

            return declarationResult;
        }

        private DeclarationResult HandleExpressionAsValue(DeclarationResult declarationResult)
        {
            if (declarationResult.ExpressionResults == null)
            {
                return declarationResult;
            }
            _generalEvaluator.ArithmeticExpressionResolved += (sender, args) =>
            {
                declarationResult.Value = args.Result.ResultValue.ToString();
            };
            _generalEvaluator.StringExpressionResolved += (sender, args) =>
            {
                declarationResult.Value = args.Result.Value;
            };
            _generalEvaluator.ExecuteExpression(declarationResult.ExpressionResults);
         

            return declarationResult;
        }
    }
}
