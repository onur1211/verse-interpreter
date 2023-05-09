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
        public override int VisitFunction_call([NotNull] Verse.Function_callContext context)
        {
            throw new NotImplementedException();
        }

        public override int VisitParam_call_item([NotNull] Verse.Param_call_itemContext context)
        {
            throw new NotImplementedException();
        }
    }
}
