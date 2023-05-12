using Antlr4.Runtime.Tree;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class DeclarationVisitor : AbstractVerseVisitor<DeclarationResult>
    {
        public DeclarationVisitor(ApplicationState applicationState) : base(applicationState)
        {
        }

        public override DeclarationResult VisitDeclaration([Antlr4.Runtime.Misc.NotNull] Verse.DeclarationContext context)
        {
            string name = context.ID().GetText();
            string type = context.INTTYPE().GetText();
            Nullable<int> value = null;
            ITerminalNode valueNode = context.INT();
            if (valueNode != null)
            {
                value = int.Parse(valueNode.GetText());
            }

            return new DeclarationResult()
            {
                Name = name,
                TypeName = type,
                Value = value,  
            };
        }

        private void AddVariableToState(DeclarationResult result)
        {
        }
    }
}
