using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Lexer
{
    public enum TokenType
    {
        TokenAssignment,
        TokenInt,
        TokenString,
        TokenEqual,
        TokenColon,
        TokenSemiColon,
        TokenPlus,
        TokenMinus,
        TokenDivision,
        TokenMultiplication,
        TokenChoice,
        TokenOpenBracket,
        TokenCloseBracket,
        TokenComma,
        TokenLiteral,
        TokenIdentifier,
        TokenOperator,
    }
}
