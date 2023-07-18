using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors
{
    // EXAM_UPDATED
    public class TypeConstructorVisitor : AbstractVerseVisitor<CustomType>
    {
        public TypeConstructorVisitor(ApplicationState applicationState) : base(applicationState)
        {
        }

        public override CustomType VisitConstructor_body([NotNull] Verse.Constructor_bodyContext context)
        {
            var constructorName = context.ID().GetText();
            if(!ApplicationState.Types.Any(x => x.Value.ConstructorName == constructorName))
            {
                throw new InvalidOperationException($"The specified constructor \"{constructorName}\" is unkown!");
            }

            var fetchedType = ApplicationState.Types[constructorName];
            return fetchedType.GetInstance();
        }
    }
}
