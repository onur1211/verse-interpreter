using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.lib.ParseVisitors.Expressions
{
	public class LogicalExpressionVisitor : AbstractVerseVisitor<LogicalExpression>
	{
		private readonly Lazy<ExpressionVisitor> _expressionVisitor;
		private LogicalExpression _logicalExpression;
		public LogicalExpressionVisitor(ApplicationState applicationState,
										Lazy<ExpressionVisitor> expressionVisitor) : base(applicationState)
		{
			_expressionVisitor = expressionVisitor;
			_logicalExpression = new LogicalExpression();
		}

		public override LogicalExpression VisitLogical_expression([NotNull] Verse.Logical_expressionContext context)
		{
			var current = new LogicalExpression();
			while(current.Next != null)
			{
				current = current.Next;
			}

			var and = context.COMMA();
			var or = context.CHOICE();
			var not = context.NOT();
			if(and.Length > 0)
			{
				current.LogicalOperator = LogicalOperators.AND;
			}
			if (or.Length > 0)
			{
				current.LogicalOperator = LogicalOperators.OR;
			}
			if (not.Length > 0)
			{
				current.IsNegated = true;
			}
			current.Expressions = _expressionVisitor.Value.Visit(context.expression());

			var nextExpression = context.logical_expression();
			if (nextExpression.Length > 0)
			{
				// Can actually never be more than element
				current.Next = Visit(nextExpression.First());
			}

			return current;
		}
	}
}
