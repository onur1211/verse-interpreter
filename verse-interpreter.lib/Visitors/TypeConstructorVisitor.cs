using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    // EXAM_UPDATED
    public class TypeConstructorVisitor : AbstractVerseVisitor<object>
    {
        public TypeConstructorVisitor(ApplicationState applicationState) : base(applicationState)
        {
        }

        public override object VisitType_constructor([NotNull] Verse.Type_constructorContext context)
        {
            var identifiers = context.ID();
            var variableName = identifiers[0];
            var constructorName = identifiers[1].GetText();
            if (!ApplicationState.Types.ContainsKey(constructorName))
            {
                throw new InvalidOperationException("The specified type doesnt exist!");
            }

            return base.VisitType_constructor(context);
        }
    }
}
