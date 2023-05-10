using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            throw new NotImplementedException();
        }

        public override ExpressionResult VisitFactor([NotNull] Verse.FactorContext context)
        {
            throw new NotImplementedException();
        }

        public override ExpressionResult VisitTerm([NotNull] Verse.TermContext context)
        {
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
