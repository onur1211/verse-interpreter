﻿using Antlr4.Runtime;
using Microsoft.Extensions.DependencyInjection;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.ResultObjects;
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
using verse_interpreter.lib.Data.ResultObjects.Validators;
using verse_interpreter.lib.Parser.ValueDefinitionParser;
using verse_interpreter.lib.Data.CustomTypes;
using verse_interpreter.lib.ParseVisitors.Functions;
using verse_interpreter.lib.ParseVisitors.Types;
using verse_interpreter.lib.ParseVisitors.Expressions;
using verse_interpreter.lib.ParseVisitors.Choice;
using verse_interpreter.lib.Evaluation.Evaluators.ForEvaluation;
using verse_interpreter.lib.Data.Variables;

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

			string libraryPath = "..\\..\\..\\..\\verse-interpreter.lib\\StandardLibrary.verse";
			this.LoadStandardLibrary(libraryPath);

			ParserTreeGenerator generator = new ParserTreeGenerator(_errorListener);
			var inputCode = options.Code != null ? options.Code :
				options.Path != null ? _reader.ReadFileToEnd(options.Path) :
				throw new ArgumentException("You have to specify either the path or add code!");

			var parseTree = generator.GenerateParseTree(inputCode);
			var mainVisitor = _services.GetRequiredService<MainVisitor>();
			mainVisitor.VisitProgram(parseTree);
			var manager = mainVisitor.ApplicationState.CurrentScope.LookupManager;
			//Console.ReadKey();
		}

		private void RunWithErrorHandling(string[] args)
		{
			try
			{
				var options = GetPath(args);
				if (options.Code == null && options.Path == null)
				{
					return;
				}
				_services = BuildService();

				string libraryPath = "..\\..\\..\\..\\..\\verse-interpreter.lib\\StandardLibrary.verse";
				this.LoadStandardLibrary(libraryPath);

				ParserTreeGenerator generator = new ParserTreeGenerator(_errorListener);

				var inputCode = options.Code != null ? options.Code :
					options.Path != null ? _reader.ReadFileToEnd(options.Path) :
					throw new ArgumentException("You have to specify either the path or add code!");

				var parseTree = generator.GenerateParseTree(inputCode);
				var mainVisitor = _services.GetRequiredService<MainVisitor>();
				mainVisitor.VisitProgram(parseTree);
				var manager = mainVisitor.ApplicationState.CurrentScope.LookupManager;
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Error: " + ex.Message);
				Console.ResetColor();
			}
		}

		private void LoadStandardLibrary(string libraryPath)
		{
			ParserTreeGenerator generator = new ParserTreeGenerator(_errorListener);
			var inputCode = _reader.ReadFileToEnd(libraryPath);
			var parseTree = generator.GenerateParseTree(inputCode);
			var mainVisitor = _services.GetRequiredService<MainVisitor>();
			mainVisitor.VisitProgram(parseTree);
		}

		private CommandLineOptions GetPath(string[] args)
		{
			CommandLineOptions options = new CommandLineOptions();

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
				.AddSingleton<MainVisitor>()
				.AddTransient<TypeInferencer>()
				.AddTransient<IEvaluator<ArithmeticExpression, List<List<ExpressionResult>>>, ArithmeticEvaluator>()
				.AddTransient<IEvaluator<StringExpression, List<List<ExpressionResult>>>, StringExpressionEvaluator>()
				.AddTransient<IEvaluator<ComparisonExpression, List<List<ExpressionResult>>>, ComparisonEvaluator>()
				.AddTransient<IEvaluator<ForExpression, ForResult>, ForEvaluator>()
				.AddTransient<IEvaluator<bool, IfParseResult>, IfEvaluator>()
				.AddTransient<IValidator<List<List<ExpressionResult>>>, ExpressionValidator>()
				.AddTransient<ParameterValidator>()
				.AddTransient<CustomTypeFactory>()
				.AddTransient<ExpressionValidator>()
				.AddTransient<DeclarationParser>()
				.AddTransient<TypeMemberVisitor>()
				.AddTransient<ValueDefinitionVisitor>()
				.AddTransient<CollectionParser>()
				.AddTransient<EvaluatorWrapper>()
				.AddTransient<TypeHandlingWrapper>()
				.AddTransient<FunctionWrapper>()
				.AddTransient<PrimaryRuleParser>()
				.AddTransient<ParameterParser>()
				.AddTransient<FunctionCallPreprocessor>()
				.AddSingleton<GeneralEvaluator>()
				.AddTransient<BodyParser>()
				.AddSingleton<FunctionCallVisitor>()
				.AddTransient<IfExpressionVisitor>()
				.AddTransient<ForVisitor>()
				.AddTransient<ChoiceVisitor>()
				.AddTransient<ChoiceArrayIndexingVisitor>()
				.AddTransient<ArrayVisitor>()
				.AddTransient<PropertyResolver>()
				.AddTransient<PredefinedFunctionInitializer>()
				.AddTransient<PredefinedFunctionEvaluator>()
				.AddTransient<ExpressionValueParser>()
				.AddTransient<FunctionFactory>()
				.AddTransient<FilterApplyer>()
				.AddTransient<LogicalExpressionVisitor>()
				.AddLazyResolution()
				.BuildServiceProvider();

			return services;
		}
	}
}