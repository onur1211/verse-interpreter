using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public partial class MainVisitor : VerseBaseVisitor<int>
    {

        public override int VisitFunction_body([NotNull] Verse.Function_bodyContext context)
        {
            throw new NotImplementedException();
        }

        public override int VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            var children = context.children;
            return base.VisitChildren(context);
        }

        public override int VisitFunction_param([NotNull] Verse.Function_paramContext context)
        {
            throw new NotImplementedException();
        }

        public override int VisitParam_def_item([NotNull] Verse.Param_def_itemContext context)
        {
            throw new NotImplementedException();
        }
    }
}
