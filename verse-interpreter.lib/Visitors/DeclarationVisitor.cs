using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class DeclarationVisitor : AbstractVerseVisitor<DeclarationResult>
    {
        public DeclarationVisitor(ApplicationState applicationState) : base(applicationState)
        {
        }

        public override DeclarationResult VisitDeclaration([Antlr4.Runtime.Misc.NotNull] Verse.DeclarationContext context)
        {
            return base.VisitDeclaration(context);
        }
    }
}
