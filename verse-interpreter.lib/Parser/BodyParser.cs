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
                var spacedBodies =  GetSpacedBody(spacedBody.spaced_body(), blocks);

                return spacedBodies;
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
            blocks.Add(context.block());

            var inlineBody = context.inline_body();
            if (inlineBody != null)
            {
                return GetInlineBody(context.inline_body(), blocks);
            }

            throw new NotImplementedException("The specified body type is not yet implemented");
        }

        private List<BlockContext> GetSpacedBody(Spaced_bodyContext context, List<BlockContext> blocks)
        {
            if (context == null)
            {
                return blocks;
            }
            blocks.Add(context.block());

            var spacedBody = context.spaced_body();

            if (spacedBody != null)
            {
                return GetSpacedBody(context.spaced_body(), blocks);
            }

           return blocks;
        }
    }
}
