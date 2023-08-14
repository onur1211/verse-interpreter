using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
		private readonly ValueDefinitionVisitor valueDefinitionVisitor;

		public ChoiceVisitor(ApplicationState applicationState,
							 ChoiceArrayIndexingVisitor indexingVisitor,
							 ValueDefinitionVisitor valueDefinitionVisitor) : base(applicationState)
		{
			_indexingVisitor = indexingVisitor;
			this.valueDefinitionVisitor = valueDefinitionVisitor;
		}

		public override ChoiceResult VisitChoice_rule([NotNull] Verse.Choice_ruleContext context)
		{
			return GenerateChoiceResult(context, new());
		}

		private ChoiceResult GenerateChoiceResult(Verse.Choice_ruleContext context, ChoiceResult result)
		{
			ParseChoices(context, result);
			ParseLiterals(context, result);

			foreach (var childNodes in context.choice_rule())
			{
				result.Next = new ChoiceResult();
				GenerateChoiceResult(childNodes, result.Next);
			}

			return result;
		}

		private ChoiceResult ParseChoices([NotNull] Verse.Choice_ruleContext context, ChoiceResult result)
		{
			if (context == null ||
				context.value_definition() == null)
			{
				return result;
			}

			var arrayIndex = _indexingVisitor.Visit(context.value_definition());
			if (arrayIndex == null)
			{
				return result;
			}
			result.IndexingResults.Add(arrayIndex);

			return result;
		}

		private ChoiceResult ParseLiterals([NotNull] Verse.Choice_ruleContext context, ChoiceResult result)
		{
			if (context == null ||
				context.value_definition() == null)
			{
				return result;
			}
			var declarationResult = valueDefinitionVisitor.Visit(context?.value_definition());
			if (declarationResult.TypeName == "false?")
			{
				return result;
			}

			result.Literals.Add(Converter.VariableConverter.Convert(declarationResult));
			return result;
		}
	}
}
