using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace verse_interpreter.lib.Lexer
{
    public class Lexer
    {
        public List<Token> Tokenize(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input), "Error: The input was null!");
            }

            List<Token> tokens = new List<Token>();
            int currentPosition = 0;

            while (currentPosition < input.Length)
            {
                // Handle operators and punctuation
                switch (input[currentPosition])
                {
                    case '=':

                        if (currentPosition + 1 < input.Length && input[currentPosition + 1] == '=')
                        {
                            currentPosition++;
                            tokens.Add(new Token("==", TokenType.TokenEqual));
                            break;
                        }

                        tokens.Add(new Token("=", TokenType.TokenAssignment));
                        break;

                    case '+':
                        tokens.Add(new Token("+", TokenType.TokenPlus));
                        break;

                    case '-':
                        tokens.Add(new Token("-", TokenType.TokenMinus));
                        break;

                    case '/':
                        tokens.Add(new Token("/", TokenType.TokenDivision));
                        break;

                    case '*':
                        tokens.Add(new Token("*", TokenType.TokenMultiplication));
                        break;

                    case ',':
                        tokens.Add(new Token(",", TokenType.TokenComma));
                        break;

                    case ':':

                        if (currentPosition + 1 < input.Length && input[currentPosition + 1] == '=')
                        {
                            currentPosition++;
                            tokens.Add(new Token(":=", TokenType.TokenAssignment));
                            break;
                        }

                        tokens.Add(new Token(":", TokenType.TokenColon));
                        break;

                    case ';':
                        tokens.Add(new Token(";", TokenType.TokenSemiColon));
                        break;

                    case '(':
                        tokens.Add(new Token("(", TokenType.TokenOpenBracket));
                        break;

                    case ')':
                        tokens.Add(new Token(")", TokenType.TokenCloseBracket));
                        break;

                    default:

                        if (char.IsWhiteSpace(input[currentPosition]))
                        {
                            break;
                        }

                        tokens.Add(new Token(input[currentPosition].ToString(), TokenType.TokenLiteral));
                        break;
                }

                currentPosition++;
            }

            return tokens;
        }
    }
}
