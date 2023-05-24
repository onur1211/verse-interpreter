using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;

namespace verse_interpreter.lib.Visitors
{
    public class FunctionDeclarationVisitor : AbstractVerseVisitor<Function>{

        private readonly FunctionParser functionDeclarationParser;
        private readonly BodyParser bodyParser;

        public FunctionDeclarationVisitor(ApplicationState applicationState,
                                          FunctionParser functionDeclarationParser,
                                          BodyParser bodyParser) : base(applicationState)
        {
            this.functionDeclarationParser = functionDeclarationParser;
            this.bodyParser = bodyParser;
        }

        public override Function VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            var name = context.ID();
            var type = context.type();
            var parameter = context.function_param().param_def_item();
            var functionDeclarationResult = new Function()
            {
                FunctionName = name.GetText(),
                ReturnType = type.GetText(),
            };
            var parameterResult = functionDeclarationParser.GetDefintionParameters(parameter);
            functionDeclarationResult.Parameters = parameterResult.Parameters;
            functionDeclarationResult.FunctionBody = bodyParser.GetBody(context.body());
            functionDeclarationResult.LookupManager.AddFunction(functionDeclarationResult);
            return functionDeclarationResult;
        }
    }
}
