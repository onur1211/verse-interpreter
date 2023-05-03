using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Parser
{
    public class Parser
    {
        public void GenerateParseTree(string input)
        {
            AntlrInputStream inputStream = new AntlrInputStream(input);
            VerseLexer lexer = new VerseLexer(inputStream);
            VerseParser parser = new VerseParser(new CommonTokenStream(lexer));
            TestVisitor visitor = new TestVisitor();
            var test = parser.Context;
        }
    }
}
