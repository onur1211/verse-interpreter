using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.Functions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluation.FunctionEvaluator;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Extensions;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Visitors;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.ParseVisitors.Functions
{
	public class FunctionCallVisitor : AbstractVerseVisitor<FunctionCallResult>
	{
		private readonly ParameterParser _functionParser;
		private readonly GeneralEvaluator _evaluator;
		private readonly FunctionCallPreprocessor _functionCallPreprocessor;
		private readonly PredefinedFunctionEvaluator _functionEvaluator;
		private readonly FunctionFactory _functionFactory;

		public FunctionCallVisitor(ApplicationState applicationState,
								   ParameterParser functionParser,
								   GeneralEvaluator evaluator,
								   FunctionCallPreprocessor functionCallPreprocessor,
								   PredefinedFunctionEvaluator functionEvaluator,
								   FunctionFactory functionFactory) : base(applicationState)
		{
			_functionParser = functionParser;
			_evaluator = evaluator;
			ApplicationState.CurrentScope.LookupManager.VariableBound += _evaluator.Propagator.HandleVariableBound!;
			_functionCallPreprocessor = functionCallPreprocessor;
			_functionEvaluator = functionEvaluator;
			_functionFactory = functionFactory;
		}

		public event EventHandler<FunctionRequestedExecutionEventArgs>? FunctionRequestedExecution;

		private ArithmeticExpression? ArithmeticExpression { get; set; }
		private StringExpression? StringExpression { get; set; }
		private ForExpression? ForExpression { get; set; }
		private Variable? Variable { get; set; }
		private bool WasValueResolved { get; set; }

		public FunctionCall Test { get; set; }

		public override FunctionCallResult VisitFunction_call([NotNull] Verse.Function_callContext context)
		{
			ClearResultSet();
			var functionName = context.ID().GetText();
			var parameters = _functionParser.GetCallParameters(context.param_call_item());
			if (TryExecutePredefinedFunction(functionName, context))
			{
				return null!;
			}

			ApplicationState.CurrentScopeLevel += 1;
			//Console.WriteLine($"Recursion depth: {ApplicationState.CurrentScopeLevel - 2}");

			var functionCall = PrepareFunctionForExecution(functionName, parameters);
			Test = functionCall;
			SetApplicationState(functionCall);

			if (!WasValueResolved)
			{
				ApplicationState.Scopes.Remove(ApplicationState.CurrentScopeLevel);
				ApplicationState.CurrentScopeLevel -= 1;
				return new FunctionCallResult()
				{
					ArithmeticExpression = ArithmeticExpression,
					StringExpression = StringExpression,
					ForExpression = ForExpression,
					WasValueResolved = WasValueResolved,
					Variable = Variable,
					IsVoid = functionCall.Function.ReturnType == "void",
				};
			}
			FunctionRequestedExecution?.Invoke(this, new FunctionRequestedExecutionEventArgs(functionCall));
			ApplicationState.Scopes.Remove(ApplicationState.CurrentScopeLevel);

			ApplicationState.CurrentScopeLevel -= 1;
			//Console.WriteLine($"Recursion depth: {ApplicationState.CurrentScopeLevel - 1}");
			if (!CheckIfReturnedValueMatchesType(functionCall.Function))
			{
				throw new InvalidTypeException();
			}

			var test = new FunctionCallResult()
			{
				ArithmeticExpression = ArithmeticExpression,
				StringExpression = StringExpression,
				ForExpression = ForExpression,
				WasValueResolved = true,
				Variable = Variable,
				IsVoid = functionCall.Function.ReturnType == "void",
			};

			return test;
		}

		private void ClearResultSet()
		{
			ForExpression = null!;
			ArithmeticExpression = null!;
			StringExpression = null!;
			WasValueResolved = true;
			Variable = null!;
		}

		private bool CheckIfReturnedValueMatchesType(Function function)
		{
			if (function.ReturnType == "void")
			{
				return true;
			}

			if (ForExpression != null)
			{
				return function.ReturnType == "collection";
			}
			if (ArithmeticExpression != null)
			{
				return function.ReturnType == "int";
			}
			if (StringExpression != null)
			{
				return function.ReturnType == "string";
			}
			if (Variable != null)
			{
				if (Variable.Value == ValueObject.False)
				{
					WasValueResolved = false;
					return true;
				}

				return function.ReturnType == Variable.Value.TypeData.Name;
			}

			return true;
		}

		private bool TryExecutePredefinedFunction(string functionName, Verse.Function_callContext context)
		{
			if (ApplicationState.PredefinedFunctions.ContainsWhere(x => x.FunctionName == functionName))
			{
				var test = _functionParser.GetCallParameters(context.param_call_item());
				_functionEvaluator.Execute(functionName, test);
				return true;
			}

			return false;
		}

		private FunctionCall PrepareFunctionForExecution(string functionName, FunctionParameters parameters)
		{
			var function = _functionFactory.GetFunctionInstance(functionName);
			var functionCall = new FunctionCall(parameters, function);
			_functionCallPreprocessor.FunctionCallValidator.FalseDetected += (sender, args) =>
			{
				WasValueResolved = false;
			};
			_functionCallPreprocessor.TryBuildExecutableFunction(functionCall);

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
			//Console.WriteLine($"Current depth ArithmeticExpression {ApplicationState.CurrentScopeLevel}. Name:{Test.Function.FunctionName} Return Type:{Test.Function.ReturnType}");
			ArithmeticExpression = arithmeticExpression;
		}

		public void OnResultEvaluated(StringExpression stringExpression)
		{
			//Console.WriteLine($"Current depth StringExpression {ApplicationState.CurrentScopeLevel}. Name:{Test.Function.FunctionName} Return Type:{Test.Function.ReturnType}");
			StringExpression = stringExpression;
		}

		public void OnResultEvaluated(ForExpression forExpression)
		{
			//Console.WriteLine($"Current depth FOREXPRESSION {ApplicationState.CurrentScopeLevel}. Name:{Test.Function.FunctionName} Return Type:{Test.Function.ReturnType}");
			ForExpression = forExpression;
		}

		public void OnVariableResolved(Variable variable)
		{
            //Console.WriteLine($"Current depth VARIABLE      {ApplicationState.CurrentScopeLevel}. Name:{Test.Function.FunctionName} Return Type:{Test.Function.ReturnType}");
            this.Variable = variable;
		}

		public void OnResultEvaluated(ExpressionWithNoValueFoundEventArgs eventArgs)
		{
			WasValueResolved = false;
		}
	}
}