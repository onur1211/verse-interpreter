using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Extensions;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.ParseVisitors.Choice
{
	public class ChoiceVisitor : AbstractVerseVisitor<ChoiceResult>
	{
		private readonly ChoiceArrayIndexingVisitor _indexingVisitor;

		public ChoiceVisitor(ApplicationState applicationState, ChoiceArrayIndexingVisitor indexingVisitor) : base(applicationState)
		{
			_indexingVisitor = indexingVisitor;
		}

		public override ChoiceResult VisitChoice_rule([NotNull] Verse.Choice_ruleContext context)
		{
			ChoiceResult result = new ChoiceResult();
			result.IndexingResults.Add(_indexingVisitor.Visit(context.value_definition().array_index()));

			return ParseChoices(context.multi_choice_rule(), result);
		}

		private ChoiceResult ParseChoices(Verse.Multi_choice_ruleContext context, ChoiceResult result)
		{
			if (context == null || context.value_definition() == null)
			{
				return result;
			}

			var arrayIndex = _indexingVisitor.Visit(context.value_definition().array_index());
			result.IndexingResults.Add(arrayIndex);
			return ParseChoices(context.multi_choice_rule(), result);
		}
	}
}
