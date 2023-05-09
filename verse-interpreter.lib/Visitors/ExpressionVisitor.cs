using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public partial class MainVisitor
    {
        public override int VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public override int VisitFactor([NotNull] Verse.FactorContext context)
        {
            throw new NotImplementedException();
        }

        public override int VisitTerm([NotNull] Verse.TermContext context)
        {
            throw new NotImplementedException();
        }

        public override int VisitPrimary([NotNull] Verse.PrimaryContext context)
        {
            throw new NotImplementedException();
        }

        public override int VisitOperator([NotNull] Verse.OperatorContext context)
        {
            throw new NotImplementedException();
        }
    }
}
