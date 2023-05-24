using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.IO;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.Visitors
{
    public class MainVisitor : AbstractVerseVisitor<object>
    {
        private readonly DeclarationVisitor _declarationVisitor;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly FunctionWrapper _functionWrapper;
        private readonly TypeHandlingWrapper _typeHandlingWrapper;
        private readonly EvaluatorWrapper _baseEvaluator;
        private readonly BackpropagationEventSystem _backPropagator;
        private readonly GeneralEvaluator _generalEvaluator;

        public MainVisitor(ApplicationState applictationState,
                           DeclarationVisitor declarationVisitor,
                           ExpressionVisitor expressionVisitor,
                           FunctionWrapper functionWrapper,
                           TypeHandlingWrapper typeHandlingWrapper,
                           EvaluatorWrapper baseEvaluator,
                           BackpropagationEventSystem backPropagator,
                           GeneralEvaluator generalEvaluator) : base(applictationState)
        {
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
            _functionWrapper = functionWrapper;
            _typeHandlingWrapper = typeHandlingWrapper;
            _baseEvaluator = baseEvaluator;
            _backPropagator = backPropagator;
            _generalEvaluator = generalEvaluator;
            ApplicationState.CurrentScope.LookupManager.VariableBound += _backPropagator.HandleVariableBound!;
        }

        public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            var declaredVariable = context.Accept(_declarationVisitor);
            ApplicationState.CurrentScope.AddScopedVariable(declaredVariable);
            return null!;
        }

        public override object VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            var res = _expressionVisitor.Visit(context);
            _expressionVisitor.Clean();

            if(ApplicationState.CurrentScopeLevel == 1)
            {
                _generalEvaluator.ArithmeticExpressionResolved += (x, y) =>
                {
                    Printer.PrintResult(y.Result.ResultValue.ToString()!);
                };
                _generalEvaluator.StringExpressionResolved += (x, y) =>
                {
                    Printer.PrintResult(y.Result.Value);
                };
            }

            _generalEvaluator.ExecuteExpression(res);
            return null!;
        }

        public override object VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            var res = _functionWrapper.FunctionDeclarationVisitor.Visit(context);
            ApplicationState.CurrentScope.AddFunction(res);
            return null!;
        }

        public override object VisitFunction_call([NotNull] Verse.Function_callContext context)
        {
            var functionCallItem = _functionWrapper.FunctionCallVisitor.Visit(context);
            _functionWrapper.FunctionCallPreprocessor.BuildExecutableFunction(functionCallItem);
            ApplicationState.CurrentScopeLevel += 1;

            ApplicationState.Scopes.Add(ApplicationState.CurrentScopeLevel, functionCallItem.Function);
            ApplicationState.CurrentScope.AddFunction(functionCallItem.Function);

            _generalEvaluator.ArithmeticExpressionResolved += (x, y) =>
            {
                
            };
            _generalEvaluator.StringExpressionResolved += (x, y) =>
            {

            };

            functionCallItem.Function.FunctionBody.ForEach(x => x.Accept(this));

            ApplicationState.Scopes.Remove(ApplicationState.CurrentScopeLevel);
            ApplicationState.CurrentScopeLevel -= 1;
            return null!;
        }

        public override object VisitType_header([NotNull] Verse.Type_headerContext context)
        {
            var novelType = _typeHandlingWrapper.TypeDefinitionVisitor.Visit(context);
            this.ApplicationState.Types.Add(novelType.Name, novelType);
            return null!;
        }

        public override object VisitType_member_definition([NotNull] Verse.Type_member_definitionContext context)
        {
            this._typeHandlingWrapper.TypeMemberVisitor.Visit(context);
            return null!;
        }
    }
}
