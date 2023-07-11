using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.Functions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluation.FunctionEvaluator;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Factories;
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

		public FunctionCallVisitor(ApplicationState applicationState,
								   FunctionParser functionParser,
								   GeneralEvaluator evaluator,
								   FunctionCallPreprocessor functionCallPreprocessor,
								   DeclarationVisitor declarationVisitor,
								   PredefinedFunctionEvaluator functionEvaluator,
								   FunctionFactory functionFactory) : base(applicationState)
		{
			_functionParser = functionParser;
			_evaluator = evaluator;
			ApplicationState.CurrentScope.LookupManager.VariableBound += _evaluator.Propagator.HandleVariableBound!;
			_functionCallPreprocessor = functionCallPreprocessor;
			_declarationVisitor = declarationVisitor;
			_functionEvaluator = functionEvaluator;
			_functionFactory = functionFactory;
		}

		public event EventHandler<FunctionRequestedExecutionEventArgs>? FunctionRequestedExecution;

		private ArithmeticExpression ArithmeticExpression { get; set; }
		private StringExpression StringExpression { get; set; }

		public override FunctionCallResult VisitFunction_call([NotNull] Verse.Function_callContext context)
		{
			var functionName = context.ID().GetText();
			var parameters = _functionParser.GetCallParameters(context.param_call_item());
			ApplicationState.CurrentScopeLevel += 1;
			if (ApplicationState.PredefinedFunctions.Count(x => x.FunctionName == functionName) >= 1)
			{
				_functionEvaluator.Execute(functionName, context.param_call_item());
				return null!;
			}

			var functionCall = PrepareFunctionForExecution(functionName, parameters);

			SetApplicationState(functionCall);
			FunctionRequestedExecution?.Invoke(this, new FunctionRequestedExecutionEventArgs(functionCall));

			ApplicationState.CurrentScopeLevel -= 1;
			return new FunctionCallResult()
			{
				ArithmeticExpression = ArithmeticExpression,
				StringExpression = StringExpression,
			};
		}

		public override FunctionCallResult VisitDeclaration(Verse.DeclarationContext context)
		{
			var variable = _declarationVisitor.VisitDeclaration(context);
			ApplicationState.CurrentScope.LookupManager.AddVariable(variable);
			return null!;
		}

		private FunctionCall PrepareFunctionForExecution(string functionName, FunctionParameters parameters)
		{
			var function = _functionFactory.GetFunctionInstance(functionName);
			var functionCall = new FunctionCall(parameters, function);
			_functionCallPreprocessor.BuildExecutableFunction(functionCall);

			return functionCall;
		}

		private void SetApplicationState(FunctionCall functionCall)
		{
			ApplicationState.Scopes.Add(ApplicationState.CurrentScopeLevel, new CurrentScope(ApplicationState.CurrentScopeLevel)
			{
				LookupManager = functionCall.Function.LookupManager
			});
		}

		public void OnResultEvaluated(ArithmeticExpression arithmeticExpression)
		{
			this.ArithmeticExpression = arithmeticExpression;
		}

		public void OnResultEvaluated(StringExpression stringExpression)
		{
			this.StringExpression = stringExpression;	
		}
	}
}
