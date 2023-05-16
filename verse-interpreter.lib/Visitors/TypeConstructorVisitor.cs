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

        public override object VisitConstructor_body([NotNull] Verse.Constructor_bodyContext context)
        {
            context.ID();
            return base.VisitConstructor_body(context);
        }
    }
}
