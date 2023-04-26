using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Lexer;
using Sprache;

namespace verse_interpreter.lib.Parser
{
    public class Parser
    {
        public void Parse(Token[] tokens)
        {
            if(tokens.Length == 0)
            {
                throw new InvalidOperationException("You can't parse an empty token sequence!");
            }

            foreach(var token in tokens)
            {
            }
        }

    }
}
