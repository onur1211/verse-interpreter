using Antlr4.Runtime.Misc;
using System.Security.Cryptography;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;
using static verse_interpreter.lib.Grammar.Verse;

namespace verse_interpreter.lib.ParseVisitors
{
	public class ExpressionVisitor : AbstractVerseVisitor<List<List<ExpressionResult>>>
	{
		private List<List<ExpressionResult>> Expressions
		{
			get { return _stack.Peek(); }
			set
			{
				if (_stack.Count > 0)
				{
					_stack.Pop();
				}

				_stack.Push(value);
			}
		}
		private Stack<List<List<ExpressionResult>>> _stack;
		private readonly PrimaryRuleParser _primaryRuleParser;

		public ExpressionVisitor(ApplicationState applicationState,
								 PrimaryRuleParser primaryRuleParser) : base(applicationState)
		{
			_stack = new Stack<List<List<ExpressionResult>>>();
			ExpressionTerminalVisited += TerminalNodeVisitedCallback;
			_primaryRuleParser = primaryRuleParser;
		}

		private event EventHandler<ExpressionTerminalVisited> ExpressionTerminalVisited = null!;
		public event EventHandler<ExpressionParsedSucessfullyEventArgs> ExpressionParsedSucessfully = null!;

		public override List<List<ExpressionResult>> VisitExpression([Antlr4.Runtime.Misc.NotNull] Verse.ExpressionContext context)
		{
			if(ApplicationState.CurrentScopeLevel > _stack.Count)
			{
				_stack.Push(new List<List<ExpressionResult>>());
			}

			// After every recursive call, a new sublist ist created to differentiate between the "scopes" of the expression --> see brackets
			Expressions.Add(new List<ExpressionResult>());

			// When all child nodes have been visited, an event is triggered containing all the unevaluated expressions as the arguments.
			//ExpressionParsedSucessfully?.Invoke(this, new ExpressionParsedSucessfullyEventArgs(_expressions));
			VisitChildren(context);

			return _stack.Pop();
		}

		public void Clean()
		{
			Expressions = new List<List<ExpressionResult>>();
		}

		public override List<List<ExpressionResult>> VisitTerm([Antlr4.Runtime.Misc.NotNull] Verse.TermContext context)
		{
			return base.VisitTerm(context);
		}

		private void TerminalNodeVisitedCallback(object? sender, ExpressionTerminalVisited? args)
		{
			if (Expressions.Count == 0)
			{
				Expressions.Add(new List<ExpressionResult>());
			}
			Expressions.Last().Add(args!.ExpressionResult);
		}

		public override List<List<ExpressionResult>> VisitPrimary([NotNull] Verse.PrimaryContext context)
		{
			var expressionContext = context.expression();
			if (expressionContext != null)
			{
				this.VisitExpression(expressionContext);
				Expressions.Add(new List<ExpressionResult>());
				return null;
			}
			var expressionResult = _primaryRuleParser.ParsePrimary(context);
			// When the instance is finalized the event is triggered to append it to the final result set
			this.ExpressionTerminalVisited?.Invoke(this, new ExpressionTerminalVisited(expressionResult));
			return base.VisitChildren(context);
		}

		public override List<List<ExpressionResult>> VisitOperator([NotNull] Verse.OperatorContext context)
		{
			var operatorResult = new ExpressionResult
			{
				Operator = context.GetText(),
			};

			this.ExpressionTerminalVisited?.Invoke(this, new ExpressionTerminalVisited(operatorResult));
			return base.VisitChildren(context);
		}
	}
}