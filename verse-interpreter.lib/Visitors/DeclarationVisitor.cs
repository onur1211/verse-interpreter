using Antlr4.Runtime.Tree;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class DeclarationVisitor : AbstractVerseVisitor<DeclarationResult>
    {
        private ExpressionVisitor _expressionVisitor;

        public DeclarationVisitor(ApplicationState applicationState,
                                  ExpressionVisitor expressionVisitor) : base(applicationState)
        {
            _expressionVisitor = expressionVisitor;
        }

        public override DeclarationResult VisitDeclaration([Antlr4.Runtime.Misc.NotNull] Verse.DeclarationContext context)
        {
            var declarationType = context.children[1];
            switch (declarationType.GetText())
            {
                case ":":
                    return ParseBringToScopeOperator(context);

                case ":=":
                    throw new NotImplementedException();

                case "=":
                    return ParseGiveValueOperator(context);

                default:
                    throw new NotImplementedException();
            }

        }

        private DeclarationResult ParseBringToScopeOperator(Verse.DeclarationContext context)
        {
            string name = context.ID().GetText();
            string type = context.INTTYPE().GetText();

            return new DeclarationResult()
            {
                Name = name,
                TypeName = type,
            };
        }

        private DeclarationResult ParseGiveValueOperator(Verse.DeclarationContext context)
        {
            string name = context.ID().GetText();
            Nullable<int> value = null;
            ITerminalNode valueNode = context.INT();
            if (valueNode != null)
            {
                value = int.Parse(valueNode.GetText());
            }
            var expression = context.expression().Accept(_expressionVisitor);
            return new DeclarationResult() { };
        }
    }
}
