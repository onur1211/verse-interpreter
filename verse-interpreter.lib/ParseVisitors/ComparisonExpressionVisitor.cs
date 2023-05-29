using verse_interpreter.lib.Data.ResultObjects;
using Z.Expressions.Compiler;

namespace verse_interpreter.lib.ParseVisitors;

public class ComparisonExpressionVisitor : AbstractVerseVisitor<ComparisonExpressionResult>
{
    public ComparisonExpressionVisitor(ApplicationState applicationState) : base(applicationState)
    {
    }
}