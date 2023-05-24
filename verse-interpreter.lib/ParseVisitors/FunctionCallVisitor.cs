using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.lib.ParseVisitors
{
    public class FunctionCallVisitor : AbstractVerseVisitor<FunctionCallItem>
    {
        private readonly FunctionParser _functionParser;

        public FunctionCallVisitor(ApplicationState applicationState,
                                   FunctionParser functionParser) : base(applicationState)
        {
            _functionParser = functionParser;
        }

        public override FunctionCallItem VisitFunction_call([NotNull] Verse.Function_callContext context)
        {
            var functionName = context.ID();
            var parameters = _functionParser.GetCallParamters(context.param_call_item());
            var body = ApplicationState.CurrentScope.LookupManager.GetFunction(functionName.GetText());

            return new FunctionCallItem(parameters, body);
        }
    }
}
