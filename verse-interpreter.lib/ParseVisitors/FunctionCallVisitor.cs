using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluation.FunctionEvaluator;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.Visitors;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.ParseVisitors
{
    public class FunctionCallVisitor : AbstractVerseVisitor<FunctionCallResult>
    {
        private readonly FunctionParser _functionParser;
        private readonly GeneralEvaluator _evaluator;
        private readonly FunctionCallPreprocessor _functionCallPreprocessor;
        private readonly DeclarationVisitor _declarationVisitor;
        private readonly PredefinedFunctionEvaluator _functionEvaluator;
        private readonly IfExpressionVisitor _ifVisitor;
        private readonly ExpressionVisitor _expressionVisitor;
        private readonly List<FunctionCallResult> _results;

        public FunctionCallVisitor(ApplicationState applicationState,
                                   FunctionParser functionParser,
                                   GeneralEvaluator evaluator,
                                   FunctionCallPreprocessor functionCallPreprocessor,
                                   DeclarationVisitor declarationVisitor,
                                   PredefinedFunctionEvaluator functionEvaluator,
                                   IfExpressionVisitor ifVisitor,
                                   ExpressionVisitor expressionVisitor) : base(applicationState)
        {
            _functionParser = functionParser;
            _evaluator = evaluator;
            ApplicationState.CurrentScope.LookupManager.VariableBound += _evaluator.Propagator.HandleVariableBound!;
            _evaluator.ArithmeticExpressionResolved += ArithmeticExpressionResolvedCallback;
            _evaluator.StringExpressionResolved += StringExpressionResolvedCallback;
            _functionCallPreprocessor = functionCallPreprocessor;
            _declarationVisitor = declarationVisitor;
            _functionEvaluator = functionEvaluator;
            _ifVisitor = ifVisitor;
            _expressionVisitor = expressionVisitor;
            _results = new List<FunctionCallResult>();
        }

        private void StringExpressionResolvedCallback(object? sender, StringExpressionResolvedEventArgs e)
        {
            _results.Add(new FunctionCallResult()
            {
                StringExpression = e.Result
            });
        }

        private void ArithmeticExpressionResolvedCallback(object? sender, ArithmeticExpressionResolvedEventArgs e)
        {
            _results.Add(new FunctionCallResult()
            {
                ArithmeticExpression = e.Result
            });
        }

        public override FunctionCallResult VisitFunction_call([NotNull] Verse.Function_callContext context)
        {
            var functionName = context.ID().GetText();
            var parameters = _functionParser.GetCallParameters(context.param_call_item());

            if (ApplicationState.PredefinedFunctions.Count(x => x.FunctionName == functionName) >= 1)
            {
                _functionEvaluator.Execute(functionName, context.param_call_item());
                return null!;
            }

            var body = ApplicationState.CurrentScope.LookupManager.GetFunction(functionName);
            var functionCallItem = new FunctionCall(parameters, body);

            _functionCallPreprocessor.BuildExecutableFunction(functionCallItem);
            ApplicationState.CurrentScopeLevel += 1;
            ApplicationState.Scopes.Add(ApplicationState.CurrentScopeLevel, functionCallItem.Function);
            ApplicationState.CurrentScope.AddFunction(functionCallItem.Function);

            //_results.AddRange(functionCallItem.Function.FunctionBody.Select(statements => statements.Accept(this)).ToList());
            foreach (var statement in functionCallItem.Function.FunctionBody)
            {
                _results.Add(statement.Accept(this));
            }
            _results.RemoveAll(x => x == null);
            ParseValueToTopScopedVariable(functionCallItem);

            ApplicationState.Scopes.Remove(ApplicationState.CurrentScopeLevel);
            ApplicationState.CurrentScopeLevel -= 1;

            if (_results.Count == 0 && functionCallItem.Function.ReturnType != "void")
            {
                throw new InvalidOperationException(
                    $"The function with the return type \"{functionCallItem.Function.ReturnType}\" has to return a value!");
            }
            if (functionCallItem.Function.ReturnType == "void")
            {
                return null!;
            }

            return _results.Last();
        }

        public override FunctionCallResult VisitExpression(Verse.ExpressionContext context)
        {
            var expression = _expressionVisitor.VisitExpression(context);
            _evaluator.ExecuteExpression(expression);
            return null!;
        }

        public override FunctionCallResult VisitIf_block(Verse.If_blockContext context)
        {
            var blocks = _ifVisitor.Visit(context);

            foreach (var block in blocks)
            {
                var result = block.Accept(this);
            }

            return null!;
        }

        public override FunctionCallResult VisitBlock(Verse.BlockContext context)
        {
            return VisitChildren(context);
        }

        public override FunctionCallResult VisitChoice_rule(Verse.Choice_ruleContext context)
        {
            throw new NotImplementedException();
        }

        public override FunctionCallResult VisitDeclaration(Verse.DeclarationContext context)
        {
            var variable = _declarationVisitor.VisitDeclaration(context);
            ApplicationState.CurrentScope.LookupManager.AddVariable(variable);
            return null!;
        }

        public override FunctionCallResult VisitType_member_access(Verse.Type_member_accessContext context)
        {
            throw new NotImplementedException();
        }

        public override FunctionCallResult VisitType_member_definition(Verse.Type_member_definitionContext context)
        {
            throw new NotImplementedException();
        }

        private void ParseValueToTopScopedVariable(FunctionCall calle)
        {
            var topLevelScope = ApplicationState.Scopes[ApplicationState.CurrentScopeLevel - 1];

            foreach (var variable in calle.Function.LookupManager.GetAllVariables())
            {
                if (topLevelScope.LookupManager.IsVariable(variable.Name) &&
                    variable.HasValue())
                {
                    topLevelScope.LookupManager.UpdateVariable(variable);
                }
            }
        }
    }
}
