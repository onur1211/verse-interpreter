using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib
{
    public class ParserListener : IParseTreeListener
    {
        public void EnterEveryRule([NotNull] ParserRuleContext ctx)
        {
            Console.WriteLine("Entered rule");
        }

        public void ExitEveryRule([NotNull] ParserRuleContext ctx)
        {
            Console.WriteLine("Exited rule");
        }

        public void VisitErrorNode([NotNull] IErrorNode node)
        {
            throw new NotImplementedException();
        }

        public void VisitTerminal([NotNull] ITerminalNode node)
        {
            Console.WriteLine(node.ToStringTree());
        }
    }
}
