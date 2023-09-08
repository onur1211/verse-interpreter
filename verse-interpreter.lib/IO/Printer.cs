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
            Console.Write("array(");
            if (collection.Values.Count == 0)
            {
                Console.Write(")");
                return;
            }
            var last = collection.Values.Last();
			foreach (var element in collection.Values)
            {
                if (element.Value.IntValue != null)
                {
					Console.Write($"{element.Value.IntValue}");
				}
                if (element.Value.StringValue != null)
                {
                    Console.Write($"{element.Value.StringValue}");
                }
                if (element != last)
                {
                    Console.Write(",");
                }
                else
                {
                    Console.Write("");
                }
            }
            Console.Write(")");
            Console.WriteLine("\n");
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
	}
}
