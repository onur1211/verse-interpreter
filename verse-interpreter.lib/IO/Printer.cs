using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.Variables;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.Lookup;

namespace verse_interpreter.lib.IO
{
    public static class Printer
    {
        public static void PrintResult(string result)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("VERSE CODE RESULT: ");
            Console.ResetColor();
            Console.WriteLine(result);
        }

        public static void PrintResult(FunctionCallResult result)
        {
            if (result == null && !result.WasValueResolved)
            {
                return;
            }

            if(result.ArithmeticExpression != null)
            {
                PrintResult(result.ArithmeticExpression.ResultValue.ToString());
                return;
            }
            if(result.StringExpression != null)
            {
                PrintResult(result.StringExpression.Value);
                return;
            }
            if (result.ForExpression != null)
            {
                PrintResult(result.ForExpression.Collection!);
                return;
			}
            if (result.Variable != null)
            {
                PrintResult(result.Variable);
                return;
            }

            throw new NotImplementedException();
        }

		public static void PrintResult(VerseCollection collection)
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine("VERSE CODE RESULT: ");
			Console.ResetColor();
			Console.WriteLine(StringifyCollectionContent(collection));
		}

		public static void PrintResult(ArithmeticExpression arithmeticExpression)
        {
            if(arithmeticExpression.PostponedExpression != null)
            {
                return;
            }

            PrintResult(arithmeticExpression.ResultValue.ToString()!);
        }

        public static void PrintResult(StringExpression stringExpression)
        {
            if(stringExpression.PostponedExpression != null)
            {
                return;
            }

            PrintResult(stringExpression.Value);
        }

        public static void PrintResult(Variable variable)
        {
			if (variable.Value == ValueObject.False)
			{
				PrintResult("false?");
			}

			switch (variable.Value)
			{
				case { IntValue: not null }:
                    PrintResult(variable.Value.IntValue.ToString()!);
					break;
				case { StringValue: not null }:
					PrintResult(variable.Value.StringValue.ToString()!);
					break;
                case { CollectionVariable: not null }:
                    PrintResult(variable.Value.CollectionVariable);
                    break;
                case { Choice: not null }:
                    PrintResult(variable.Value.Choice);
                    break;
			}
		}

		public static void PrintResult(Choice choice)
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine("VERSE CODE RESULT: ");
			Console.ResetColor();
            Console.Write("(");
            var choices = choice.AllChoices();
            var last = choices.LastOrDefault();
			foreach (var element in choices)
            {
                if (element.ValueObject.IntValue != null)
                {
					Console.Write($"{element.ValueObject.IntValue}");
				}
				if (element.ValueObject.StringValue != null)
				{
					Console.Write($"{element.ValueObject.StringValue}");
				}
                if (element != last)
                {
                    Console.Write("|");
                }
			}
            Console.Write(")");
		}

		private static string StringifyCollectionContent(VerseCollection collection)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("array( ");

			if (collection.Values.Count == 0)
			{
				stringBuilder.Append(")");
				Console.WriteLine(stringBuilder.ToString());
				return stringBuilder.ToString();
			}

			var last = collection.Values.Last();

			foreach (var element in collection.Values)
			{
				if (!element.HasValue())
				{
					stringBuilder.Append($"{element.Name}:{element.Value.TypeData.Name}");
				}
				if (element.Value.IntValue != null)
				{
					stringBuilder.Append($"{element.Value.IntValue}");
				}
				if (element.Value.StringValue != null)
				{
					stringBuilder.Append($"{element.Value.StringValue}");
				}
				if (element.Value.TypeData.Name == "false?")
				{
					stringBuilder.Append($"{element.Value.TypeData.Name}");
				}
				if (element.Value.CollectionVariable != null)
				{
					stringBuilder.Append(StringifyCollectionContent(element.Value.CollectionVariable));
				}
				if (element.Value.Choice != null)
				{
					stringBuilder.Append(StringifyChoiceContent(element.Value.Choice));
				}
				if (element != last)
				{
					stringBuilder.Append(", ");
				}
				else
				{
					stringBuilder.Append(" ");
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private static string StringifyChoiceContent(Choice choice)
		{
			StringBuilder sb = new StringBuilder();
            sb.Append("(");
            var choices = choice.AllChoices();
            var last = choices.LastOrDefault();
            foreach (var element in choices)
            {
                if (element.ValueObject.IntValue != null)
                {
                    sb.Append($"{element.ValueObject.IntValue}");
                }
                if (element.ValueObject.StringValue != null)
                {
                    sb.Append($"{element.ValueObject.StringValue}");
                }
                if (element != last)
                {
                    sb.Append("|");
                }
            }
            sb.Append(")");
			return sb.ToString();
        }

		public static void PrintDebugInformation(LookupManager manager)
		{
			for (int i = 0; i < 5; i++)
			{
				Console.WriteLine(".");
			}

			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine("DEBUG INFO:");
			Console.WriteLine("'x:int': declared variable, but no value assigned.");
            Console.WriteLine("'error': error case. Can not be printed.");
            Console.WriteLine();
			Console.ResetColor();

            foreach (var variable in manager.GetAllVariables())
			{
				switch (true)
				{
					case true when !variable.HasValue():
                        Console.WriteLine($"Name: {variable.Name}, Type: {variable.Value.TypeData.Name}");
                        break;

                    case true when variable.Value!.IntValue != null:
						Console.WriteLine($"Name: {variable.Name}, Type: {variable.Value.TypeData.Name}, Value: {variable.Value.IntValue}");
						break;

					case true when variable.Value.StringValue != null:
						Console.WriteLine($"Name: {variable.Name}, Type: {variable.Value.TypeData.Name}, Value: {variable.Value.StringValue}");
						break;

					case true when variable.Value.CollectionVariable != null:
						Console.WriteLine($"Name: {variable.Name}, Type: {variable.Value.TypeData.Name}, Value: {StringifyCollectionContent(variable.Value.CollectionVariable)}");
						break;

                    case true when variable.Value.Choice != null:
                        Console.WriteLine($"Name: {variable.Name}, Type: {variable.Value.TypeData.Name}, Value: {StringifyChoiceContent(variable.Value.Choice)}");
                        break;

                    case true when variable.Value.TypeData.Name == "false?":
						Console.WriteLine($"Name: {variable.Name}, Type: {variable.Value.TypeData.Name}");
						break;

					default:
						Console.WriteLine("error");
						break;
				}
			}
    }
	}
}
