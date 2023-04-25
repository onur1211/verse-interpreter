using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Lexer
{
    public class Token
    {
        public Token(string value, TokenType tokenType)
        {
            this.TokenType = tokenType;
            this.Value = value;
        }

        public string Value { get; private set; }

        public TokenType TokenType { get; private set; }
    }
}
