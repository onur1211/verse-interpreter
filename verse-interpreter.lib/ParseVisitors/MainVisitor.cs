using Antlr4.Runtime.Misc;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Evaluation.EvaluationManagement;
using verse_interpreter.lib.EventArguments;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.IO;
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
		private readonly IfExpressionVisitor _ifExpressionVisitor;

		public MainVisitor(ApplicationState applicationState,
						   DeclarationVisitor declarationVisitor,
						   ExpressionVisitor expressionVisitor,
						   FunctionWrapper functionWrapper,
						   TypeHandlingWrapper typeHandlingWrapper,
						   GeneralEvaluator generalEvaluator,
						   IfExpressionVisitor ifExpressionVisitor) : base(applicationState)
		{
			_declarationVisitor = declarationVisitor;
			_expressionVisitor = expressionVisitor;
			_functionWrapper = functionWrapper;
			_typeHandlingWrapper = typeHandlingWrapper;
			_functionWrapper.FunctionCallVisitor.FunctionRequestedExecution += FunctionRequestedExecutionCallback;
			_generalEvaluator = generalEvaluator;
			ApplicationState.CurrentScope.LookupManager.VariableBound +=
				_generalEvaluator.Propagator.HandleVariableBound!;
			_ifExpressionVisitor = ifExpressionVisitor;
			_generalEvaluator.ArithmeticExpressionResolved += ArithmeticExpressionResolved;
			_generalEvaluator.StringExpressionResolved += StringExpressionResolved;
		}

		private void StringExpressionResolved(object? sender, StringExpressionResolvedEventArgs e)
		{
			if (ApplicationState.CurrentScopeLevel > 1)
			{
				_functionWrapper.FunctionCallVisitor.OnResultEvaluated(e.Result);
				return;
			}

      if (declaredVariable.Value.TypeName != "undefined")
      {
         ApplicationState.CurrentScope.AddScopedVariable(declaredVariable);
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

		private void FunctionRequestedExecutionCallback(object? sender, FunctionRequestedExecutionEventArgs e)
		{
			foreach (var block in e.FunctionCall.Function.FunctionBody)
			{
				foreach(var child in block.children)
				{
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

    public override object VisitIf_block(Verse.If_blockContext context)
    {
           var result =  _ifExpressionVisitor.Visit(context);
           foreach (var value in result)
           {
               value.Accept(this);
           }
           return null!;
        }
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
			if(result == null || result.IsVoid)
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

		public override IPrintable VisitIf_block(Verse.If_blockContext context)
		{
			var result = _ifExpressionVisitor.Visit(context);
			foreach (var value in result)
			{
				value.Accept(this);
			}
			return null!;
		}
	}
}