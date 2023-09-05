using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.Functions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Extensions;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.ParseVisitors;
using verse_interpreter.lib.ParseVisitors.Functions;

namespace verse_interpreter.lib.Evaluation.FunctionEvaluator
{
	public class FunctionCallEvaluator
	{
		private readonly ApplicationState _applicationState;
		private readonly FunctionFactory _functionFactory;
		private readonly FunctionCallPreprocessor _functionCallPreprocessor;
		private readonly PredefinedFunctionEvaluator _predefinedFunctionEvaluator;
		private Stack<ReturnValueManager> _returnValueManagerStack;

		public FunctionCallEvaluator(ApplicationState applicationState,
									 FunctionFactory functionFactory,
									 FunctionCallPreprocessor functionCallPreprocessor,
									 PredefinedFunctionEvaluator predefinedFunctionEvaluator)
		{
			_applicationState = applicationState;
			_functionFactory = functionFactory;
			_functionCallPreprocessor = functionCallPreprocessor;
			_predefinedFunctionEvaluator = predefinedFunctionEvaluator;
			_returnValueManagerStack = new Stack<ReturnValueManager>();
		}

		public event EventHandler<FunctionRequestedExecutionEventArgs>? FunctionRequestedExecution;

		public ReturnValueManager ReturnValueManager { get { return _returnValueManagerStack.Peek(); } }

		public FunctionCallResult CallFunction(FunctionParameters parameters, string functionName)
		{
			_returnValueManagerStack.Push(new ReturnValueManager());
			if (IsNoValueResolved())
			{
				_applicationState.Scopes.Remove(_applicationState.CurrentScopeLevel);
				_applicationState.CurrentScopeLevel -= 1;
				return _returnValueManagerStack.Pop().BuildResult();
			}
			if (TryExecutePredefinedFunction(parameters, functionName))
			{
				return new FunctionCallResult()
				{
					IsVoid = true,
				};
			}

			_applicationState.CurrentScopeLevel += 1;
			//Console.WriteLine($"Recursion depth: {_applicationState.CurrentScopeLevel - 1}");
			var callItem = PrepareFunctionForExecution(parameters, functionName);

			if (TryExecuteChoice(parameters, functionName))
			{
				// Return the functionCallResult with a Choice as value
			}

			SetApplicationState(callItem);
			FunctionRequestedExecution?.Invoke(this, new FunctionRequestedExecutionEventArgs(callItem));

			_applicationState.Scopes.Remove(_applicationState.CurrentScopeLevel);
			_applicationState.CurrentScopeLevel -= 1;
			//Console.WriteLine($"Recursion depth: {_applicationState.CurrentScopeLevel}");

			var endResult = _returnValueManagerStack.Pop().BuildResult();
			endResult.IsVoid = _applicationState.Functions[functionName].ReturnType == "void";
			return endResult;
		}

		private bool TryExecuteChoice(FunctionParameters parameters, string functionName)
		{
			if (!parameters.Parameters.Any(x => x.Value.Choice != null))
			{
				return false;
			}

			foreach (var parameter in parameters.Parameters)
			{
				if (parameter.Value.Choice != null)
				{
					foreach (var element in parameter.Value.Choice.AllChoices())
					{
						parameter.Value = element.ValueObject;
						var first = PrepareFunctionForExecution(parameters, functionName);
						// Execute
					}
				}
				else
				{
					// do some normal execution
				}
			}

			return true;
		}

		private FunctionCall PrepareFunctionForExecution(FunctionParameters parameters, string functionName)
		{
			var function = _functionFactory.GetFunctionInstance(functionName);
			var functionCall = new FunctionCall(parameters, function);
			_functionCallPreprocessor.FunctionCallValidator.FalseDetected += (sender, args) =>
			{
				_returnValueManagerStack.Peek().OnResultEvaluated(args);
			};

			if (!_functionCallPreprocessor.TryBuildExecutableFunction(functionCall))
			{
			}

			return functionCall;
		}

		private void SetApplicationState(FunctionCall functionCall)
		{
			_applicationState.Scopes.Add(_applicationState.CurrentScopeLevel, new CurrentScope(_applicationState.CurrentScopeLevel)
			{
				LookupManager = functionCall.Function.LookupManager
			});
		}

		private bool TryExecutePredefinedFunction(FunctionParameters parameters, string functionName)
		{
			if (_applicationState.PredefinedFunctions.ContainsWhere(x => x.FunctionName == functionName))
			{
				_predefinedFunctionEvaluator.Execute(functionName, parameters);
				return true;
			}

			return false;
		}

		private bool IsNoValueResolved()
		{
			if (_returnValueManagerStack.Count == 0)
			{
				return false;
			}
			var returnedValue = _returnValueManagerStack.Peek().BuildResult();

			if (returnedValue.Variable != null)
			{
				if (returnedValue.Variable.Value == ValueObject.False)
				{
					_returnValueManagerStack.Peek().WasValueResolved = false;
					return true;
				}
			}

			return false;
		}
	}
}
