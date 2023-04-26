using System.Text.RegularExpressions;

namespace verse_interpreter.lib.Lexer
{
    public class Lexer
    {

        public List<Token> Tokenize(string input)
        {
            List<Token> tokens = new List<Token>();
            Dictionary<string, string> keywords = new Dictionary<string, string>()
            {
                { "int", "TokenInt" },
                { "float", "TokenFloat" }
            };

            List<Tuple<string, string>> patterns = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(@"\bint\b", "TokenNumber"),
                new Tuple<string, string>(@"\bfloat\b", keywords["float"]),
                new Tuple<string, string>(@"\b[a-zA-Z]+\b", "TokenIdentifier"),
                new Tuple<string, string>(@"\b\d+(\.\d+)?\b", "TokenLiteral"),
                new Tuple<string, string>(@"[+\-*/]", "TokenOperator"),
                new Tuple<string, string>(@"[=]", "TokenEqual"),
                new Tuple<string, string>(@"[;]", "TokenSemiColon"),
                new Tuple<string, string>(@":", "TokenColon"),
                new Tuple<string, string>(@"\(", "TokenOpenBracket"),
                new Tuple<string, string>(@"\)", "TokenCloseBracket")
            };

            string tokenPattern = string.Join("|", patterns.ConvertAll(pattern => $"(?<{Regex.Replace(pattern.Item2, @"\W", "")}>{pattern.Item1})"));
            var regex = new Regex(tokenPattern);
            var parsedString = input.Split(' ');
            MatchCollection matches = regex.Matches(string.Join(" ", parsedString));

            foreach (var match in matches)
            {
                var castedMatch = match as Match;
                foreach (var groupName in regex.GetGroupNames())
                {
                    if (castedMatch.Groups[groupName].Success && castedMatch.Groups[groupName].Name != "0" &&
                        castedMatch.Groups[groupName].Name != "1")
                    {
                        tokens.Add(ParseToken(castedMatch, groupName));
                    }
                }
            }

            return tokens;
        }

        private Token ParseToken(Match match, string groupName)
        {
            TokenType type;
            int number;
            bool isValid = Enum.TryParse<TokenType>(match.Groups[groupName].Name, out type);
            bool isNumber = int.TryParse(match.Value, out number);
            if (!isValid)
            {
                throw new InvalidTokenException("The specified token was not found");
            }
            if (isNumber)
            {
                return new Token(number.ToString(), TokenType.TokenInt);
            }

            return new Token(match.Value, type);
        }
    }
}