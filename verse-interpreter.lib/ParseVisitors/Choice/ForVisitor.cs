using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors.Choice
{
	public class ForVisitor : AbstractVerseVisitor<ForResult>
	{
		private readonly ChoiceVisitor _choiceVisitor;

		public ForVisitor(ApplicationState applicationState, ChoiceVisitor choiceVisitor) : base(applicationState)
		{
			_choiceVisitor = choiceVisitor;
		}

		public override ForResult VisitNarrowingBody([NotNull] Verse.NarrowingBodyContext context)
		{
			return base.VisitNarrowingBody(context);
		}

		public override ForResult VisitChoiceBody([NotNull] Verse.ChoiceBodyContext context)
		{
			var result = _choiceVisitor.Visit(context);
			return base.VisitChoiceBody(context);
		}
	}
}
