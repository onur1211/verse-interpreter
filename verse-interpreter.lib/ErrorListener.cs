using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Sharpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib
{
    public class ErrorListener : BaseErrorListener
    {
        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            Console.WriteLine($"Unexpected token \"{offendingSymbol}\" located at Line{line}:{charPositionInLine}");
            base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }
    }
}
