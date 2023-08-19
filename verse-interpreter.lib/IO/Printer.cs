using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.ResultObjects;
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
            if (result == null || !result.WasValueResolved)
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
                return;
			}

            throw new NotImplementedException();
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
    }
}
