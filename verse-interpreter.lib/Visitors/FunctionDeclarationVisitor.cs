using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class FunctionDeclarationVisitor : AbstractVerseVisitor<FunctionDeclarationResult>
    {
        public FunctionDeclarationVisitor(ApplicationState applicationState) : base(applicationState)
        {
        }

        public override FunctionDeclarationResult VisitFunction_body([NotNull] Verse.Function_bodyContext context)
        {
            throw new NotImplementedException();
        }

        public override FunctionDeclarationResult VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            var name = context.ID();
            var type = context.INTTYPE();
            var parameter = context.function_param();

            foreach (var param in parameter.children)
            {
                if(param.GetText() == "(" ||  param.GetText() == ")")
                {
                    continue;
                }

                param.Accept(this);
            }
            return base.VisitChildren(context);
        }

        public override FunctionDeclarationResult VisitParam_def_item([NotNull] Verse.Param_def_itemContext context)
        {
            throw new NotImplementedException();
        }
    }
}
