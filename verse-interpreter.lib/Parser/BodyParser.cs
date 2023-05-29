using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;
using static verse_interpreter.lib.Grammar.Verse;

namespace verse_interpreter.lib.Parser
{
    public class BodyParser
    {

        public List<BlockContext> GetBody(Verse.BodyContext bodyContext)
        {
            List<BlockContext> blocks = new List<BlockContext>();
            var body = GetBlock(bodyContext, blocks);
            if (body == null)
            {
                throw new ArgumentNullException("The body is null!");
            }

            return body;
        }

        private List<BlockContext> GetBlock(BodyContext context, List<BlockContext> blocks)
        {
            var inlineBody = context.inline_body();
            var spacedBody = context.spaced_body();

            if(inlineBody != null)
            {
                blocks.Add(inlineBody.block());
                return GetInlineBody(inlineBody.inline_body(), blocks);
            }
            if(spacedBody != null)
            {
                blocks.Add(spacedBody.block());
                return GetSpacedBody(spacedBody.spaced_body(), blocks);
            }

            GetSpacedBody(context.spaced_body(), blocks);
            GetInlineBody(context.inline_body(), blocks);

            return blocks;
        }

        private List<BlockContext> GetInlineBody(Inline_bodyContext context, List<BlockContext> blocks)
        {
            if (context == null)
            {
                return blocks;
            }

            var inlineBody = context.inline_body();
            if (inlineBody != null)
            {
                blocks.Add(inlineBody.block());
                return GetInlineBody(context.inline_body(), blocks);
            }
            if(context != null && inlineBody != null)
            {
                blocks.Add(context.block());
            }

            throw new NotImplementedException("The specified body type is not yet implemented");
        }

        private List<BlockContext> GetSpacedBody(Spaced_bodyContext context, List<BlockContext> blocks)
        {
            if (context == null)
            {
                return blocks;
            }

            var inlineBody = context.spaced_body();
            if (inlineBody != null)
            {
                blocks.Add(inlineBody.block());
                return GetSpacedBody(context.spaced_body(), blocks);
            }
            if (context != null && inlineBody == null)
            {
                blocks.Add(context.block());
            }

           return blocks;
        }
    }
}
