using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.IO;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.lib.Evaluation.FunctionEvaluator;

public class PredefinedFunctionEvaluator
{
	private readonly ApplicationState _applicationState;

	public PredefinedFunctionEvaluator(ApplicationState applicationState, ParameterParser functionParser)
	{
		_applicationState = applicationState;
	}

	public void Execute(string functionName, FunctionParameters parameters)
	{
		var function = _applicationState.PredefinedFunctions.First(x => x.FunctionName == functionName);
		if (function.ParameterCount != parameters.ParameterCount)
		{
			throw new NotEqualArityException();
		}

		switch (function.FunctionName)
		{
			case "Print":
				ExecutePrint(functionName, parameters);
				break;
		}
	}

	private void ExecutePrint(string functionName, FunctionParameters parameters)
	{
		var function = _applicationState.PredefinedFunctions.First(x => x.FunctionName == functionName);

		if (parameters.Parameters[0].Value.StringValue != null)
		{
			Printer.PrintResult(parameters.Parameters[0].Value.StringValue);
		}
		if (parameters.Parameters[0].Value.IntValue != null)
		{
			Printer.PrintResult(parameters.Parameters[0].Value.IntValue.ToString()!);
		}
		if (parameters.Parameters[0].Value.TypeData.Name == "false?")
		{
			Printer.PrintResult("false?");
		}
		if (parameters.Parameters[0].Value.CollectionVariable != null)
		{
			Printer.PrintResult(parameters.Parameters[0].Value.CollectionVariable);
		}
		if (parameters.Parameters[0].Value.Choice != null)
		{
			Printer.PrintResult(parameters.Parameters[0].Value.Choice);
		}
	}
}