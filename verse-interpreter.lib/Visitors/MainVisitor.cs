using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class MainVisitor : VerseBaseVisitor<object>
    {
        private FunctionDeclarationVisitor _functionDeclarationVisitor;
        private DeclarationVisitor _declarationVisitor;
        private ExpressionVisitor _expressionVisitor;

        public MainVisitor(FunctionDeclarationVisitor functionDeclarationVisitor,
                           DeclarationVisitor declarationVisitor,
                           ExpressionVisitor expressionVisitor)
        {
            _functionDeclarationVisitor = functionDeclarationVisitor;
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
        }

        public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            var res = context.Accept(_declarationVisitor);
            return base.VisitChildren(context);
        }

        public override object VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            var result = context.Accept(_functionDeclarationVisitor);
            return base.VisitFunction_definition(context);
        }

        public override object VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            _expressionVisitor.Visit(context);
            return base.VisitExpression(context);
        }
    }
}
