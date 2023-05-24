using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
