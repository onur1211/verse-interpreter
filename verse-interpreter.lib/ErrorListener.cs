using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace verse_interpreter.lib
{
    public class ErrorListener : BaseErrorListener
    {
        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            Console.WriteLine($"Unexpected token \"{offendingSymbol.Text}\" located at Line {line}:{charPositionInLine}");
            //base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }
    }
}
