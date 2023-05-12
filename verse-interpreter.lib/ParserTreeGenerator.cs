using Antlr4.Runtime;
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
    public class ParserTreeGenerator
    {
        private VerseLexer _lexer;
        private Verse _parser;
        private IParserErrorListener _listener;
        private IParseTreeListener _treeListener;

        public ParserTreeGenerator(IParserErrorListener errorListener, 
                                   IParseTreeListener parseListener)
        {
            _lexer = null!;
            _parser = null!;
            _listener = errorListener;
            _treeListener = parseListener;
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
                BuildParseTree = true
            };
            _parser.AddErrorListener(_listener);
            _parser.AddParseListener(_treeListener);
            return _parser.program();
        }
    }
}
