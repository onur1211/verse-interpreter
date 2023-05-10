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
    public class FunctionExecutionVisitor : AbstractVerseVisitor<FunctionExecutionResult>
    {
        public FunctionExecutionVisitor(ApplicationState applicationState) : base(applicationState)
        {
        }

        public override FunctionExecutionResult VisitFunction_call([Antlr4.Runtime.Misc.NotNull] Verse.Function_callContext context)
        {
            return base.VisitFunction_call(context);
        }
    }
}
