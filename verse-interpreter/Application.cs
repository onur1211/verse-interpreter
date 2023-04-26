using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Lexer;

namespace verse_interpreter.exe
{
    public class Application
    {
        public void Run()
        {
            Lexer lexer = new Lexer();
            string sampleVerseCode = "test:=5; y:=1;";
           var res2= lexer.TestTokenize(sampleVerseCode);
            foreach (var token in res2)
            {
                Console.WriteLine($"TokenValue: {token.Value}  ||  TokenType: {token.TokenType.ToString()}");
            }

            Console.ReadKey();
        }
    }
}
