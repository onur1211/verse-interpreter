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
        private ApplicationState _applicationState;

        public ParserListener(ApplicationState applicationState)
        {
            _applicationState = applicationState;
        }

        public void EnterEveryRule([NotNull] ParserRuleContext ctx)
        {
            _applicationState.CurrentScopeLevel += 1;
        }

        public void ExitEveryRule([NotNull] ParserRuleContext ctx)
        {
            _applicationState.CurrentScopeLevel -= 1;
        }

        public void VisitErrorNode([NotNull] IErrorNode node)
        {
            throw new NotImplementedException();
        }

        public void VisitTerminal([NotNull] ITerminalNode node)
        {
            //Console.WriteLine(node.ToStringTree());
        }
    }
}
