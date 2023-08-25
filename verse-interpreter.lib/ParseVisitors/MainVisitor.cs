using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Expressions;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.Evaluation.Evaluators.ForEvaluation;
using verse_interpreter.lib.Evaluators;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.IO;
using verse_interpreter.lib.ParseVisitors.Choice;
using verse_interpreter.lib.ParseVisitors.Expressions;
using verse_interpreter.lib.Wrapper;

namespace verse_interpreter.lib.ParseVisitors
{
	public class MainVisitor : AbstractVerseVisitor<object>
	{
		private readonly DeclarationVisitor _declarationVisitor;
		private readonly ExpressionVisitor _expressionVisitor;
		private readonly FunctionWrapper _functionWrapper;
		private readonly TypeHandlingWrapper _typeHandlingWrapper;
		private readonly GeneralEvaluator _generalEvaluator;
		private readonly ForVisitor _forVisitor;
		private readonly ValueDefinitionVisitor _valueDefinitionVisitor;
		private readonly IfExpressionVisitor _ifExpressionVisitor;

		public MainVisitor(ApplicationState applicationState,
						   DeclarationVisitor declarationVisitor,
						   ExpressionVisitor expressionVisitor,
						   FunctionWrapper functionWrapper,
						   TypeHandlingWrapper typeHandlingWrapper,
						   GeneralEvaluator generalEvaluator,
						   ForVisitor forVisitor,
						   ValueDefinitionVisitor valueDefinitionVisitor,
						   IfExpressionVisitor ifExpressionVisitor) : base(applicationState)
		{
			_declarationVisitor = declarationVisitor;
			_expressionVisitor = expressionVisitor;
			_functionWrapper = functionWrapper;
			_typeHandlingWrapper = typeHandlingWrapper;
			_functionWrapper.FunctionCallVisitor.FunctionRequestedExecution += FunctionRequestedExecutionCallback;
			_generalEvaluator = generalEvaluator;
			_forVisitor = forVisitor;
			_valueDefinitionVisitor = valueDefinitionVisitor;
			ApplicationState.CurrentScope.LookupManager.VariableBound +=
				_generalEvaluator.Propagator.HandleVariableBound!;
			_ifExpressionVisitor = ifExpressionVisitor;
			_generalEvaluator.ArithmeticExpressionResolved += ArithmeticExpressionResolved;
			_generalEvaluator.StringExpressionResolved += StringExpressionResolved;
			_generalEvaluator.ForExpressionResolved += ForExpressionResolved;
			_generalEvaluator.ExpressionWithNoValueFound += ValueLessExpressionDetected;
		}

		private void FunctionRequestedExecutionCallback(object? sender, FunctionRequestedExecutionEventArgs e)
		{
			foreach (var block in e.FunctionCall.Function.FunctionBody)
			{
				foreach (var child in block.children)
				{
					// Ugly but due to aliases in the grammar the delegation to the value definition visitor would require
					// writing out every possible value definition sub rule as a visitor method call in order to make this work
					if (child is Verse.Value_definitionContext)
					{
						var res = VariableConverter.Convert(_valueDefinitionVisitor.Visit(child)!);
						_functionWrapper.FunctionCallVisitor.OnVariableResolved(res);
						continue;
					}
					child.Accept(this);
				}
			}
		}

		public override object VisitDeclaration([NotNull] Verse.DeclarationContext context)
		{
			var declaredVariable = context.Accept(_declarationVisitor);

			if (declaredVariable.Value.TypeData.Name != "undefined")
			{
				ApplicationState.CurrentScope.AddScopedVariable(declaredVariable);
			}
			return null!;
		}

		public override object VisitExpression([NotNull] Verse.ExpressionContext context)
		{
			var res = _expressionVisitor.Visit(context);
			_expressionVisitor.Clean();

			_generalEvaluator.ExecuteExpression(res);
			return null!;
		}

		public override object VisitFunction_definition([NotNull] Verse.Function_definitionContext context)
		{
			var res = _functionWrapper.FunctionDeclarationVisitor.Visit(context);
			ApplicationState.AddFunction(res);
			return null!;
		}

		public override FunctionCallResult VisitFunction_call([NotNull] Verse.Function_callContext context)
		{
			var result = _functionWrapper.FunctionCallVisitor.Visit(context);
			if (result == null || result.IsVoid)
			{
				return null;
			}
			Printer.PrintResult(result);
			return result;
		}

		public override object VisitType_header([NotNull] Verse.Type_headerContext context)
		{
			var novelType = _typeHandlingWrapper.TypeDefinitionVisitor.Visit(context);
			this.ApplicationState.Types.Add(novelType.Name, novelType);
			return null!;
		}

		public override object VisitType_member_definition([NotNull] Verse.Type_member_definitionContext context)
		{
			this._typeHandlingWrapper.TypeMemberVisitor.Visit(context);
			return null!;
		}

		public override object VisitIf_block(Verse.If_blockContext context)
		{
			var result = _ifExpressionVisitor.Visit(context);
			bool isSuccess = false;
			_generalEvaluator.IfExpressionResolved += (sender, args) =>
			{
				isSuccess = args.IsSuccess;
			};
			_generalEvaluator.ExecuteExpression(result);

			if (isSuccess)
			{
				// Add variables from If Head
				if (result.ScopedVariable != null)
				{
					ApplicationState.CurrentScope.AddScopedVariable(result.ScopedVariable);
				}

				foreach (var element in result.ThenBlock)
				{
					element.Accept(this);
				}
				// Remove after execution
				if (result.ScopedVariable != null)
				{
					ApplicationState.CurrentScope.LookupManager.RemoveVariable(result.ScopedVariable);
				}
			}
			else
			{
				foreach (var element in result.ElseBlock)
				{
					element.Accept(this);
				}
			}
			return null!;
		}

		public override object VisitLambdaFunc([NotNull] Verse.LambdaFuncContext context)
		{
			var res = _functionWrapper.FunctionDeclarationVisitor.Visit(context);
			ApplicationState.AddFunction(res);
			return null!;
		}

		public override object VisitFunc([NotNull] Verse.FuncContext context)
		{
			var res = _functionWrapper.FunctionDeclarationVisitor.Visit(context);
			ApplicationState.AddFunction(res);
			return null!;
		}

		public override object VisitFor_rule([NotNull] Verse.For_ruleContext context)
		{
			var forExpression = _forVisitor.Visit(context);
			_generalEvaluator.ExecuteExpression(forExpression);
			return null;
		}

		public override object VisitArray_literal([NotNull] Verse.Array_literalContext context)
		{
			return base.VisitArray_literal(context);
		}

		#region Callbacks
		private void StringExpressionResolved(object? sender, StringExpressionResolvedEventArgs e)
		{
			if (ApplicationState.CurrentScopeLevel > 1)
			{
				_functionWrapper.FunctionCallVisitor.OnResultEvaluated(e.Result);
				return;
			}

			Printer.PrintResult(e.Result);
		}

		private void ArithmeticExpressionResolved(object? sender, ArithmeticExpressionResolvedEventArgs e)
		{
			if (ApplicationState.CurrentScopeLevel > 1)
			{
				_functionWrapper.FunctionCallVisitor.OnResultEvaluated(e.Result);
				return;
			}

			Printer.PrintResult(e.Result);
		}

		private void ValueLessExpressionDetected(object? sender, ExpressionWithNoValueFoundEventArgs eventArgs)
		{
			if (ApplicationState.CurrentScopeLevel > 1)
			{
				_functionWrapper.FunctionCallVisitor.OnResultEvaluated(eventArgs);
				return;
			}

			Printer.PrintResult("false?");
		}

		private void ForExpressionResolved(object? sender, ForExpressionResolvedEventArgs eventArgs)
		{
			if (ApplicationState.CurrentScopeLevel > 1)
			{
				_functionWrapper.FunctionCallVisitor.OnResultEvaluated(eventArgs.ForExpression);
				return;
			}
		}
		#endregion
	}
}
