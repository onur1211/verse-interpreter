using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.lib.Evaluation.FunctionEvaluator;

public class PredefinedFunctionEvaluator
{
    private readonly ApplicationState _applicationState;
    private readonly FunctionParser _functionParser;

    public PredefinedFunctionEvaluator(ApplicationState applicationState, FunctionParser functionParser)
    {
        _applicationState = applicationState;
        _functionParser = functionParser;
    }

    public void Execute(string functionName, Verse.Param_call_itemContext paramCallItemContext)
    {
        var function = _applicationState.PredefinedFunctions.First(x => x.FunctionName == functionName);
        var parameter = _functionParser.GetCallParameters(paramCallItemContext);
        if (function.ParameterCount != parameter.ParameterCount)
        {
            throw new NotEqualArityException();
        }

        switch (function.FunctionName)
        {
            case "Print": ExecutePrint(functionName, paramCallItemContext);
            break;
        }
    }

    private void ExecutePrint(string functionName, Verse.Param_call_itemContext paramCallItemContext)
    {
        var function = _applicationState.PredefinedFunctions.First(x => x.FunctionName == functionName);
        var parameter = _functionParser.GetCallParameters(paramCallItemContext);

        if (parameter.Parameters[0].Value.StringValue != null)
        {
            function.StatelessFunctionCall.Invoke(parameter.Parameters[0].Value.StringValue);
        }

        if (parameter.Parameters[0].Value.IntValue != null)
        {
            function.StatelessFunctionCall.Invoke(parameter.Parameters[0].Value.IntValue.ToString());
        }

        if (parameter.Parameters[0].Value.TypeData.Name == "false?")
        {
            function.StatelessFunctionCall.Invoke("false?");
        }
    }
}