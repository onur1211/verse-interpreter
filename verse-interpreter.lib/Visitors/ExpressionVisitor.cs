using System.Diagnostics.CodeAnalysis;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class ExpressionVisitor : AbstractVerseVisitor<ExpressionResult>
    {
        public ExpressionVisitor(ApplicationState applicationState) : base(applicationState)
        {
        }

        public override ExpressionResult VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            var res = context.term().Accept(this);
            throw new NotImplementedException();
        }

        public override ExpressionResult VisitTerm([NotNull] Verse.TermContext context)
        {
            var firstprimary = context.term().factor().primary().GetText();
            var operator1 = context.@operator().GetText();
            var secondPrimary = context.factor().primary().GetText();
            throw new NotImplementedException();
        }

        public override ExpressionResult VisitPrimary([NotNull] Verse.PrimaryContext context)
        {
            throw new NotImplementedException();
        }

        public override ExpressionResult VisitOperator([NotNull] Verse.OperatorContext context)
        {
            throw new NotImplementedException();
        }
    }
}
