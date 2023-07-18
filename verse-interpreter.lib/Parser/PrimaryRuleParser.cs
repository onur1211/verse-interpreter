using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.ParseVisitors;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.Parser
{
    public class PrimaryRuleParser
    {
        public ExpressionResult ParsePrimary([Antlr4.Runtime.Misc.NotNull] Verse.PrimaryContext context)
        {
            ExpressionResult result = new ExpressionResult();
            // Checks if the there are any subexpressions --> due to brackets for instance
            // Fetches the value / identifer from the current node
            var fetchedValue = context.INT();
            var fetchedNoValue = context.NOVALUE();
            var fetchedIdentifier = context.ID();
            var fetchedMemberAccess = context.type_member_access();
            var fetchedString = context.string_rule();
            var fetchedArrayAccess = context.array_index();

            if (fetchedValue != null)
            {
                result.IntegerValue = Convert.ToInt32(fetchedValue.ToString());
                result.TypeName = "int";
                return result;
            }

            if (fetchedNoValue != null) 
            {
                result.TypeName = "false?";
                return result;
            }

            if (fetchedIdentifier != null)
            {
                result.ValueIdentifier = fetchedIdentifier.GetText();
                return result;

            }

            if (fetchedMemberAccess != null)
            {
                result.ValueIdentifier = fetchedMemberAccess.GetText();
                return result;
            }

            if(fetchedString != null)
            {
                result.StringValue = fetchedString.GetText();
                result.TypeName = "string";
                return result;
            }

            if (fetchedArrayAccess != null)
            {
                result.ValueIdentifier =  fetchedArrayAccess.GetText();
                result.TypeName = "collection";
                return result;
            }

            throw new NotImplementedException();
        }
    }
}