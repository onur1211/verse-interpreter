using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.InputProcessing;
using verse_interpreter.lib.Lexer;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.exe
{
    public class Application
    {
        public void Run()
        {
         Parser parser = new Parser();
            parser.GenerateParseTree("x:int");
        }
    }
}
