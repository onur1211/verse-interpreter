using Antlr4.Runtime.Tree;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class DeclarationVisitor : AbstractVerseVisitor<DeclarationResult>
    {
       private DeclarationParser _parser;

        public DeclarationVisitor(ApplicationState applicationState,
                                  DeclarationParser declarationParser): base(applicationState)
        {
          _parser = declarationParser;
        }

        public override DeclarationResult VisitDeclaration([Antlr4.Runtime.Misc.NotNull] Verse.DeclarationContext context)
        {
            return _parser.ParseDeclaration(context);
        }
    }
}
