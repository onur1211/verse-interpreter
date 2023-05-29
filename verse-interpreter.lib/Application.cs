using Antlr4.Runtime;
using Microsoft.Extensions.DependencyInjection;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Data.Validators;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluation.Evaluators;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Extensions;
using verse_interpreter.lib.Factories;
using verse_interpreter.lib.IO;
using verse_interpreter.lib.Parser;
using verse_interpreter.lib.ParseVisitors;
using verse_interpreter.lib.Visitors;
using verse_interpreter.lib.Wrapper;
using CommandLine;
using verse_interpreter.lib.Data.Functions;
using verse_interpreter.lib.Evaluation.FunctionEvaluator;

namespace verse_interpreter.lib
{
    public class Application
    {
        private IParserErrorListener _errorListener;
        private IServiceProvider _services;
        private FileReader _reader;

        public Application()
        {
            _errorListener = new ErrorListener();
            _services = null!;
            _reader = new FileReader();
        }

        public void Run(string[] args)
        {
            var options = GetPath(args);
            if (options.Code == null && options.Path == null)
            {
                return;
            }
            _services = BuildService();
            ParserTreeGenerator generator = new ParserTreeGenerator(_errorListener);

            var inputCode = options.Code != null ? options.Code :
                options.Path != null ? _reader.ReadFileToEnd(options.Path) :
                throw new ArgumentException("You have to specify either the path or add code!");


            var parseTree = generator.GenerateParseTree(inputCode);
            var mainVisitor = _services.GetRequiredService<MainVisitor>();
            mainVisitor.VisitProgram(parseTree);
            var manager = mainVisitor.ApplicationState.CurrentScope.LookupManager;
        }

        private CommandLineOptions GetPath(string[] args)
        {
            CommandLineOptions options = new CommandLineOptions();
            string path = null;
            CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed<CommandLineOptions>(o =>
                {
                    if (string.IsNullOrEmpty(o.Path) && string.IsNullOrEmpty(o.Code))
                    {
                        throw new ArgumentException("The path must not be null!");
                    }
                    options.Path = o.Path;
                    options.Code = o.Code;
                });

            return options;
        }

        private IServiceProvider BuildService()
        {
            var services = new ServiceCollection()
                .AddSingleton<ApplicationState>()
                .AddTransient<BackpropagationEventSystem>()
                .AddTransient<DeclarationVisitor>()
                .AddTransient<ExpressionVisitor>()
                .AddTransient<FunctionDeclarationVisitor>()
                .AddTransient<TypeDefinitionVisitor>()
                .AddTransient<TypeConstructorVisitor>()
                .AddTransient<MainVisitor>()
                .AddTransient<TypeInferencer>()
                .AddTransient<IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>>, ArithmeticEvaluator>()
                .AddTransient<IEvaluator<StringExpression, List<List<ExpressionResult>>>, StringExpressionEvaluator>()
                .AddTransient<IValidator<List<List<ExpressionResult>>>, ExpressionValidator>()
                .AddTransient<IValidator<FunctionCall>, ParameterValidator>()
                .AddTransient<ExpressionValidator>()
                .AddTransient<DeclarationParser>()
                .AddTransient<TypeMemberVisitor>()
                .AddTransient<ValueDefinitionVisitor>()
                .AddTransient<CollectionParser>()
                .AddTransient<EvaluatorWrapper>()
                .AddTransient<TypeHandlingWrapper>()
                .AddTransient<FunctionWrapper>()
                .AddTransient<PrimaryRuleParser>()
                .AddTransient<FunctionParser>()
                .AddTransient<FunctionCallPreprocessor>()
                .AddTransient<GeneralEvaluator>()
                .AddTransient<BodyParser>()
                .AddTransient<BlockParser>()
                .AddTransient<FunctionCallVisitor>()
                .AddTransient<IfExpressionVisitor>()
                .AddTransient<PropertyResolver>()
                .AddTransient<PredefinedFunctionInitializer>()
                .AddTransient<PredefinedFunctionEvaluator>()
                .AddLazyResolution()
                .BuildServiceProvider();

            return services;
        }

        private void RunWithErrorHandling(string[] args)
        {
            var inputCode = _reader.ReadFileToEnd("../../../../verse-interpreter.lib/VerseTemplate.verse");

            try
            {
                _services = BuildService();
                ParserTreeGenerator generator = new ParserTreeGenerator(_errorListener);
                var parseTree = generator.GenerateParseTree(inputCode);
                var mainVisitor = _services.GetRequiredService<MainVisitor>();
                mainVisitor.VisitProgram(parseTree);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine();
                Console.WriteLine("Code: ");
                Console.Write(inputCode);
                Console.ResetColor();
            }

            Console.ReadKey();
        }
    }
}
