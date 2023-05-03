using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib
{
    public class TestVisitor : VerseBaseVisitor<object>
    {
        public override object VisitDeclaration([NotNull] VerseParser.DeclarationContext context)
        {
            var test = context.ID();
            Console.WriteLine($"{context.ID()} is the name with the type {context.INTTYPE()}");
            return null!;
        }
    }
}
