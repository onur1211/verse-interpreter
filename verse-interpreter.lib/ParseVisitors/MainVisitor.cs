using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.IO;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.ParseVisitors
{
    public class MainVisitor : AbstractVerseVisitor<object>
    {
        private readonly DeclarationVisitor _declarationVisitor;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly FunctionWrapper _functionWrapper;
        private readonly TypeHandlingWrapper _typeHandlingWrapper;
        private readonly GeneralEvaluator _generalEvaluator;
        private readonly IfExpressionVisitor _ifExpressionVisitor;

        public MainVisitor(ApplicationState applicationState,
                           DeclarationVisitor declarationVisitor,
                           ExpressionVisitor expressionVisitor,
                           FunctionWrapper functionWrapper,
                           TypeHandlingWrapper typeHandlingWrapper,
                           GeneralEvaluator generalEvaluator,
                           IfExpressionVisitor ifExpressionVisitor) : base(applicationState)
        {
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
            _functionWrapper = functionWrapper;
            _typeHandlingWrapper = typeHandlingWrapper;
            _generalEvaluator = generalEvaluator;
            ApplicationState.CurrentScope.LookupManager.VariableBound +=
                _generalEvaluator.Propagator.HandleVariableBound!;
            _ifExpressionVisitor = ifExpressionVisitor;
        }

        public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            var declaredVariable = context.Accept(_declarationVisitor);

            if (declaredVariable.Value.TypeName != "undefined")
            {
                ApplicationState.CurrentScope.AddScopedVariable(declaredVariable);
            }

            return null!;
        }

        public override object VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            var res = _expressionVisitor.Visit(context);
            _expressionVisitor.Clean();
            if (ApplicationState.CurrentScopeLevel == 1)
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
            var result = _functionWrapper.FunctionCallVisitor.Visit(context);
            Printer.PrintResult(result);
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

        public override object VisitIf_block(Verse.If_blockContext context)
        {
           var result =  _ifExpressionVisitor.Visit(context);
           foreach (var value in result)
           {
               value.Accept(this);
           }
           return null!;
        }
    }
}
