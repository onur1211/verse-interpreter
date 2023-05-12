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
        private VerseBaseVisitor<object> _baseVisitor;
        private FunctionDeclarationVisitor _functionDeclarationVisitor;
        private DeclarationVisitor _declarationVisitor;

        public MainVisitor(FunctionDeclarationVisitor functionDeclarationVisitor, DeclarationVisitor declarationVisitor)
        {
            _baseVisitor = new VerseBaseVisitor<object>();
            _functionDeclarationVisitor = functionDeclarationVisitor;
            _declarationVisitor = declarationVisitor;
        }

        public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            context.Accept(_declarationVisitor);
            return base.VisitChildren(context);
        }
    }
}
