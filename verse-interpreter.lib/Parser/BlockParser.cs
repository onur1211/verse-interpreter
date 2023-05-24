using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Parser
{
    public class BlockParser
    {
        public void ParseBlock(Verse.BlockContext context)
        {
            List<ParserRuleContext> contexts = new List<ParserRuleContext>
            {
                context.if_block(),
                context.expression(),
                context.declaration(),
                context.function_call()
            };

        }
    }

}
