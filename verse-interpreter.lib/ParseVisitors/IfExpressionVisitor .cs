
using Antlr4.Runtime.Tree;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.lib.ParseVisitors
{
    public class IfExpressionVisitor : AbstractVerseVisitor<object>
    {
        private readonly BodyParser _parser;

        public IfExpressionVisitor(ApplicationState applicationState,
                                   BodyParser parser) : base(applicationState)
        {
            _parser = parser;
        }

        public override object VisitThen_block(Verse.Then_blockContext context)
        {
            var result = _parser.GetBody(context.body());
            return null!;
        }

        public override object VisitElse_block(Verse.Else_blockContext context)
        {
            var result = _parser.GetBody(context.body());
            return null!;
        }
    }
}


