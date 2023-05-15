using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.Grammar;
using Z.Expressions;

namespace verse_interpreter.lib.Visitors
{
    public class MainVisitor : AbstractVerseVisitor<object>
    {
        private FunctionDeclarationVisitor _functionDeclarationVisitor;
        private DeclarationVisitor _declarationVisitor;
        private ExpressionVisitor _expressionVisitor;
        private TypeDefinitionVisitor _typeDefinitionVisitor;
        private TypeInferencer _inferencer;
        private TypeConstructorVisitor _typeConstructorVisitor;
        private IEvaluator<int, List<List<ExpressionResult>>> _arithmeticEvaluator;
        private IEvaluator<string, List<List<ExpressionResult>>> _stringEvaluator;

        public MainVisitor(ApplicationState applictationState,
                           FunctionDeclarationVisitor functionDeclarationVisitor,
                           DeclarationVisitor declarationVisitor,
                           ExpressionVisitor expressionVisitor,
                           TypeDefinitionVisitor typeDefinitionVisitor,
                           TypeConstructorVisitor typeConstructorVisitor,
                           TypeInferencer typeInferencer,
                           IEvaluator<int, List<List<ExpressionResult>>> arithmeticEvaluator,
                           IEvaluator<string, List<List<ExpressionResult>>> stringEvaluator) : base(applictationState)
        {
            _functionDeclarationVisitor = functionDeclarationVisitor;
            _declarationVisitor = declarationVisitor;
            _expressionVisitor = expressionVisitor;
            _typeDefinitionVisitor = typeDefinitionVisitor; 
            _inferencer = typeInferencer;
            _typeConstructorVisitor = typeConstructorVisitor;
            _arithmeticEvaluator = arithmeticEvaluator;
            _stringEvaluator = stringEvaluator;
        }

        public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
        {
            var declaredVariable = context.Accept(_declarationVisitor);
            ApplicationState.Scopes[1].AddScopedVariable(1, _inferencer.InferGivenType(declaredVariable));
            return null!;
        }

        public override object VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
        {
            var result = context.Accept(_functionDeclarationVisitor);
            return null!;
        }

        public override object VisitExpression([NotNull] Verse.ExpressionContext context)
        {
            _expressionVisitor.ExpressionParsedSucessfully += (sender, args) =>
            {
                // The first printed result is the correct one.
                string result = HandleEvaluation(args.Expressions);
                this.PrintResult(result);
            };

            _expressionVisitor.Visit(context);
            return null!;
        }

        public override object VisitType_header([NotNull] Verse.Type_headerContext context)
        {
            ApplicationState.CurrentScopeLevel += 1;
            var novelType = _typeDefinitionVisitor.Visit(context);
            this.ApplicationState.Types.Add(novelType.Name, novelType);

            return null!;
        }

        public override object VisitConstructors([NotNull] Verse.ConstructorsContext context)
        {
            _typeConstructorVisitor.Visit(context);
            return null!;
        }

        private void PrintResult(string result)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("VERSE CODE RESULT: ");
            Console.ResetColor();
            Console.WriteLine(result);
        }

        // EXAM_UPDATED
        private string HandleEvaluation(List<List<ExpressionResult>> expressionResults)
        {
            foreach(var expressionResult in expressionResults)
            {
                foreach(var res in expressionResult)
                {
                    if(res.ValueIdentifier != null )
                    {
                        var test = ApplicationState.Scopes[1].LookupManager.GetVariableStrings(res.ValueIdentifier).Count;
                        var test2 = ApplicationState.Scopes[1].LookupManager.GetVariableInts(res.ValueIdentifier).Count;

                        return (test == test2)? throw new InvalidOperationException("Cant mix types!") : 
                            (test > test2)? _stringEvaluator.Evaluate(expressionResults) : _arithmeticEvaluator.Evaluate(expressionResults).ToString();
                    } 
                }
            }

            return null;
        }
    }
}
