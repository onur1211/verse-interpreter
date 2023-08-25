using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Data.Functions;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Parser;
using verse_interpreter.lib.Visitors;

namespace verse_interpreter.lib.ParseVisitors.Functions
{
    public class FunctionDeclarationVisitor : AbstractVerseVisitor<Function>
    {

        private readonly ParameterParser functionDeclarationParser;
        private readonly BodyParser bodyParser;

        public FunctionDeclarationVisitor(ApplicationState applicationState,
                                          ParameterParser functionDeclarationParser,
                                          BodyParser bodyParser) : base(applicationState)
        {
            this.functionDeclarationParser = functionDeclarationParser;
            this.bodyParser = bodyParser;
        }

        public override Function VisitLambdaFunc([NotNull] Verse.LambdaFuncContext context)
        {
            var name = context.ID();

            var parameter = context.param_def_item();
            var functionDeclarationResult = new Function()
            {
                FunctionName = name.GetText(),
            };
            var parameterResult = functionDeclarationParser.GetDefinitionParameters(parameter);
            functionDeclarationResult.Parameters = parameterResult.Parameters;
            functionDeclarationResult.FunctionBody = bodyParser.GetBody(context.body());

            return functionDeclarationResult;
        }

        public override Function VisitFunc([NotNull] Verse.FuncContext context)
        {
            var name = context.ID();
            var type = context.type();

            var parameter = context.function_param().param_def_item();
            var functionDeclarationResult = new Function()
            {
                FunctionName = name.GetText(),
                ReturnType = type.GetText(),
            };
            var parameterResult = functionDeclarationParser.GetDefinitionParameters(parameter);
            functionDeclarationResult.Parameters = parameterResult.Parameters;
            functionDeclarationResult.FunctionBody = bodyParser.GetBody(context.body());

            return functionDeclarationResult;
        }
    }
}
