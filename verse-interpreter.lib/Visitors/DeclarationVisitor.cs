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
        public override int VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
