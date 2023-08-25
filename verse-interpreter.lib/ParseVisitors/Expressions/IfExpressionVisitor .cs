using Antlr4.Runtime.Misc;
using CommandLine;
using System.Diagnostics;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;
using verse_interpreter.lib.ParseVisitors.Expressions;

namespace verse_interpreter.lib.ParseVisitors
{
	public class IfExpressionVisitor : AbstractVerseVisitor<IfParseResult>
	{
		private readonly BodyParser _parser;
		private readonly DeclarationParser _declarationParser;
		private readonly LogicalExpressionVisitor _expressionVisitor;

		public IfExpressionVisitor(ApplicationState applicationState,
								   BodyParser parser,
								   DeclarationParser declarationParser,
								   LogicalExpressionVisitor expressionVisitor) : base(applicationState)
		{
			_parser = parser;
			_declarationParser = declarationParser;
			_expressionVisitor = expressionVisitor;
		}

		public override IfParseResult VisitIf_block(Verse.If_blockContext context)
		{
			IfParseResult parseResult = new IfParseResult();
			parseResult.ThenBlock = ParseThenBlock(context);
			parseResult.ElseBlock = ParseElseBlock(context);
			var logicalExpression = context.logical_expression();
			var declaration = context.declaration();
			if (declaration != null)
			{
				parseResult.ScopedVariable = VariableConverter.Convert(_declarationParser.ParseDeclaration(declaration));
			}
			if (logicalExpression != null)
			{
				parseResult.LogicalExpression = _expressionVisitor.Visit(logicalExpression);
			}

			return parseResult;
		}

		public override IfParseResult VisitDeclaration([NotNull] Verse.DeclarationContext context)
		{
			return base.VisitDeclaration(context);
		}

		private List<Verse.BlockContext> ParseThenBlock(Verse.If_blockContext context)
		{
			return _parser.GetBody(context.then_block().body());
		}

		private List<Verse.BlockContext> ParseElseBlock(Verse.If_blockContext context)
		{
			var elseBlock = context.else_block();
			if( elseBlock != null )
			{
				return _parser.GetBody(context.else_block().body());
			}

			return new List<Verse.BlockContext>();
		}
	}
}
