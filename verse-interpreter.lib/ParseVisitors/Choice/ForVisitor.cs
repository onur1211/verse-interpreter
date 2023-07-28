using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.lib.ParseVisitors.Choice
{
	public class ForVisitor : AbstractVerseVisitor<ForResult>
	{
		private readonly ChoiceVisitor _choiceVisitor;
		private readonly Lazy<DeclarationParser> _declarationParser;

		public ForVisitor(ApplicationState applicationState,
						  ChoiceVisitor choiceVisitor,
						  Lazy<DeclarationParser> declarationParser) : base(applicationState)
		{
			_choiceVisitor = choiceVisitor;
			_declarationParser = declarationParser;
			_result = new ForResult();
		}

		private ForResult _result;

		public override ForResult VisitForChoice([NotNull] Verse.ForChoiceContext context)
		{
			var res = _choiceVisitor.VisitForChoice(context);
			return _result;
		}

		public override ForResult VisitForArrayIndex([NotNull] Verse.ForArrayIndexContext context)
		{
			return base.VisitForArrayIndex(context);
		}

		public override ForResult VisitForExpression([NotNull] Verse.ForExpressionContext context)
		{
			return base.VisitForExpression(context);
		}

		public override ForResult VisitFor_declaration([NotNull] Verse.For_declarationContext context)
		{
			ParseDeclarationsRecursively(context, _result);
			_result.LocalVariables.ForEach(x => ApplicationState.CurrentScope.AddScopedVariable(x));
			return _result;
		}

		private ForResult ParseDeclarationsRecursively([NotNull] Verse.For_declarationContext context, ForResult initialResult)
		{
			if (context == null)
			{
				return initialResult;
			}

			var variable = VariableConverter.Convert(_declarationParser.Value.ParseDeclaration(context.declaration()));
			initialResult.LocalVariables.Add(variable);

			var childElement = context.for_declaration();
			if (childElement != null && childElement.Any())
			{
				return ParseDeclarationsRecursively(childElement.First(), initialResult);
			}

			return initialResult;
		}
	}
}
