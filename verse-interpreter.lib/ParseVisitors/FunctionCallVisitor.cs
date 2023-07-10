using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Functions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluation.FunctionEvaluator;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Visitors;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.ParseVisitors
{
	public class FunctionCallVisitor : AbstractVerseVisitor<FunctionCallResult>
	{
		private readonly FunctionParser _functionParser;
		private readonly GeneralEvaluator _evaluator;
		private readonly FunctionCallPreprocessor _functionCallPreprocessor;
		private readonly DeclarationVisitor _declarationVisitor;
		private readonly PredefinedFunctionEvaluator _functionEvaluator;
		private readonly FunctionFactory _functionFactory;
		private readonly IfExpressionVisitor _ifVisitor;
		private readonly ExpressionVisitor _expressionVisitor;
		private readonly List<FunctionCallResult> _results;

		public FunctionCallVisitor(ApplicationState applicationState,
								   FunctionParser functionParser,
								   GeneralEvaluator evaluator,
								   FunctionCallPreprocessor functionCallPreprocessor,
								   DeclarationVisitor declarationVisitor,
								   PredefinedFunctionEvaluator functionEvaluator,
								   FunctionFactory functionFactory,
								   IfExpressionVisitor ifVisitor,
								   ExpressionVisitor expressionVisitor) : base(applicationState)
		{
			_functionParser = functionParser;
			_evaluator = evaluator;
			ApplicationState.CurrentScope.LookupManager.VariableBound += _evaluator.Propagator.HandleVariableBound!;
			_evaluator.ArithmeticExpressionResolved += ArithmeticExpressionResolvedCallback;
			_evaluator.StringExpressionResolved += StringExpressionResolvedCallback;
			_functionCallPreprocessor = functionCallPreprocessor;
			_declarationVisitor = declarationVisitor;
			_functionEvaluator = functionEvaluator;
			_functionFactory = functionFactory;
			_ifVisitor = ifVisitor;
			_expressionVisitor = expressionVisitor;
			_results = new List<FunctionCallResult>();
		}

		public event EventHandler<FunctionRequestedExecutionEventArgs> FunctionRequestedExecution;

		public override FunctionCallResult VisitFunction_call([NotNull] Verse.Function_callContext context)
		{
			var functionName = context.ID().GetText();
			var parameters = _functionParser.GetCallParameters(context.param_call_item());

			var functionCall = PrepareFunctionForExecution(functionName, parameters);

			ApplicationState.CurrentScopeLevel += 1;
			ApplicationState.Scopes.Add(ApplicationState.CurrentScopeLevel, new CurrentScope(ApplicationState.CurrentScopeLevel));
			FunctionRequestedExecution.Invoke(this, new FunctionRequestedExecutionEventArgs(functionCall));

			return null;
		}

		public override FunctionCallResult VisitChoice_rule(Verse.Choice_ruleContext context)
		{
			throw new NotImplementedException();
		}

		public override FunctionCallResult VisitDeclaration(Verse.DeclarationContext context)
		{
			var variable = _declarationVisitor.VisitDeclaration(context);
			ApplicationState.CurrentScope.LookupManager.AddVariable(variable);
			return null!;
		}

		private void StringExpressionResolvedCallback(object? sender, StringExpressionResolvedEventArgs e)
		{
			_results.Add(new FunctionCallResult()
			{
				StringExpression = e.Result
			});
		}

		private void ArithmeticExpressionResolvedCallback(object? sender, ArithmeticExpressionResolvedEventArgs e)
		{
			_results.Add(new FunctionCallResult()
			{
				ArithmeticExpression = e.Result
			});
		}

		private FunctionCall PrepareFunctionForExecution(string functionName, FunctionParameters parameters)
		{
			var function = _functionFactory.GetFunctionInstance(functionName);
			var functionCall = new FunctionCall(parameters, function);
			_functionCallPreprocessor.BuildExecutableFunction(functionCall);

			return functionCall;
		}
	}
}
