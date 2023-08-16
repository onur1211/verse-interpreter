using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.ResultObjects.Expressions;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;
using verse_interpreter.lib.ParseVisitors.Expressions;

namespace verse_interpreter.lib.ParseVisitors.Choice
{
	public class ForVisitor : AbstractVerseVisitor<ForResult>
	{
		private readonly ChoiceVisitor _choiceVisitor;
		private readonly Lazy<DeclarationParser> _declarationParser;
		private readonly ExpressionVisitor _expressionVisitor;
		private readonly PrimaryRuleParser _primaryParser;

		public ForVisitor(ApplicationState applicationState,
						  ChoiceVisitor choiceVisitor,
						  Lazy<DeclarationParser> declarationParser,
						  ExpressionVisitor expressionVisitor,
					      PrimaryRuleParser primaryParser) : base(applicationState)
		{
			_choiceVisitor = choiceVisitor;
			_declarationParser = declarationParser;
			_expressionVisitor = expressionVisitor;
			_primaryParser = primaryParser;
			_result = new ForResult();
		}

		private ForResult _result;

		public override ForResult VisitFor_rule([NotNull] Verse.For_ruleContext context)
		{
			_result = new ForResult();
			VisitChildren(context);
			return _result;
		}

		public override ForResult VisitForChoice([NotNull] Verse.ForChoiceContext context)
		{
			var res = _choiceVisitor.VisitForChoice(context);
			_result.Choices = res;
			return _result;
		}

		public override ForResult VisitForExpression([NotNull] Verse.ForExpressionContext context)
		{
			var resultSet = new ExpressionSet(_expressionVisitor.Visit(context.expression()));
			if(IsComparisionExpression(resultSet))
			{
				_result.Filters.Clear();
				_result.Filters.Add(resultSet);
			}
			return base.VisitForExpression(context);
		}

		public override ForResult VisitFor_declaration([NotNull] Verse.For_declarationContext context)
		{
			ParseDeclarationsRecursively(context, _result);
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

		private bool IsComparisionExpression(ExpressionSet expressionSet)
		{
			foreach (var expression in expressionSet.Expressions)
			{
				if (expression.Any(x => x.Operator != string.Empty && (x.Operator == ">" || x.Operator == "<" || x.Operator == "=" || x.Operator == "<=" || x.Operator == ">=")))
				{
					return true;
				}
			}

			return false;
		}
	}
}
