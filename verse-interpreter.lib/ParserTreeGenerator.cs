using Antlr4.Runtime;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib
{
    public class ParserTreeGenerator
    {
        private VerseLexer _lexer;
        private Verse _parser;
        private IParserErrorListener _listener;

        public ParserTreeGenerator(IParserErrorListener errorListener)
        {
            _lexer = null!;
            _parser = null!;
            _listener = errorListener;
        }

        public Verse.ProgramContext GenerateParseTree(string inputSequence)
        {
            if (string.IsNullOrEmpty(inputSequence))
            {
                throw new ArgumentException("The specified input was empty!");
            }

            AntlrInputStream inputStream = new(inputSequence);
            _lexer = new VerseLexer(inputStream);
            _parser = new Verse(new CommonTokenStream(_lexer))
            {
                BuildParseTree = true,
                TrimParseTree = true,
            };
            _parser.AddErrorListener(_listener);

            return _parser.program();
        }
    }
}
