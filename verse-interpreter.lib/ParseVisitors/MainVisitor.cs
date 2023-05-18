using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.Visitors
{
    public class MainVisitor : AbstractVerseVisitor<object>
    {
        private DeclarationVisitor _declarationVisitor;
        private ExpressionVisitor _expressionVisitor;
        private readonly TypeHandlingWrapper _typeHandlingWrapper;
        private readonly EvaluatorWrapper _baseEvaluator;
        private readonly BackpropagationEventSystem _backPropagator;

        public MainVisitor(ApplicationState applictationState,
                           DeclarationVisitor declarationVisitor,
                           ExpressionVisitor expressionVisitor,
                           TypeHandlingWrapper typeHandlingWrapper,
                           EvaluatorWrapper baseEvaluator,
                           BackpropagationEventSystem backPropagator) : base(applictationState)
        {
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
            _typeHandlingWrapper = typeHandlingWrapper;
            _baseEvaluator = baseEvaluator;
            _backPropagator = backPropagator;
            ApplicationState.CurrentScope.LookupManager.VariableBound += _backPropagator.HandleVariableBound!;
        }

        public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            var declaredVariable = context.Accept(_declarationVisitor);
            ApplicationState.CurrentScope.AddScopedVariable(1, declaredVariable);
            return null!;
        }

        public override object VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            var res = _expressionVisitor.Visit(context);
            var expression = _baseEvaluator.ArithmeticEvaluator.Evaluate(res);
            if (expression.PostponedExpression == null)
            {
                PrintResult(expression.ResultValue.ToString());
            }
            else
            {
                _backPropagator.AddExpression(expression);
            }
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

        private void PrintResult(string result)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("VERSE CODE RESULT: ");
            Console.ResetColor();
            Console.WriteLine(result);
        }
    }
}
