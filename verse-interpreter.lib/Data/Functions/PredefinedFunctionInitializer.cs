namespace verse_interpreter.lib.Data.Functions;

public class PredefinedFunctionInitializer
{
	public List<Function> GetPredefinedFunctions()
	{
		List<Function> functions = new List<Function>()
		{
			CreatePrintFunction(),
		};

		return functions;
	}

	public Function CreatePrintFunction()
	{
		Function print = new Function();
		print.FunctionName = "Print";
		print.Parameters.Add(new Variable("message", new ValueObject("any")));
		print.ReturnType = "void";
		return print;
	}
}