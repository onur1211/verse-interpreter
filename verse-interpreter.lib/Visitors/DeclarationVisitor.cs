using Antlr4.Runtime.Tree;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class DeclarationVisitor : AbstractVerseVisitor<DeclarationResult>
    {
        private ExpressionVisitor _expressionVisitor;
        private TypeInferencer _typeInferencer;

        public DeclarationVisitor(ApplicationState applicationState,
                                  ExpressionVisitor expressionVisitor,
                                  TypeInferencer inferencer) : base(applicationState)
        {
            _expressionVisitor = expressionVisitor;
            _typeInferencer = inferencer;
        }

        public override DeclarationResult VisitDeclaration([Antlr4.Runtime.Misc.NotNull] Verse.DeclarationContext context)
        {
            var declarationType = context.children[1];
            switch (declarationType.GetText())
            {
                case ":":
                    return ParseBringToScopeOperator(context);

                    // Needs to be updated to conform actual semantics 
                case ":=":
                    return ParseGiveValueOperator(context);

                case "=":
                    return ParseGiveValueOperator(context);

                default:
                    throw new NotImplementedException();
            }

        }

        private DeclarationResult ParseBringToScopeOperator(Verse.DeclarationContext context)
        {
            string name = context.ID().GetText();
            string type = context.type().GetText();

            return new DeclarationResult()
            {
                Name = name,
                TypeName = type,
            };
        }

        // EXAM_UPDATED
        private DeclarationResult ParseGiveValueOperator(Verse.DeclarationContext context)
        {
            return _typeInferencer.InferGivenType(context);
        }
    }
}
