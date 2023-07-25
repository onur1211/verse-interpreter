using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors.Choice
{
    public class ChoiceVisitor : AbstractVerseVisitor<ChoiceResult>
    {
		private readonly ValueDefinitionVisitor _valueDefinitionVisitor;

		public ChoiceVisitor(ApplicationState applicationState, ValueDefinitionVisitor valueDefinitionVisitor) : base(applicationState)
        {
			_valueDefinitionVisitor = valueDefinitionVisitor;
		}

        public override ChoiceResult VisitChoice_rule([NotNull] Verse.Choice_ruleContext context)
        {
            var res = _valueDefinitionVisitor.Visit(context.value_definition());
            return base.VisitChoice_rule(context);
        }
    }
}
