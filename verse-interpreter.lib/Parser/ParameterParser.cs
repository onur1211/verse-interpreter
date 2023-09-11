using verse_interpreter.lib.Converter;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.ParseVisitors;

namespace verse_interpreter.lib.Visitors
{
	public class ParameterParser
	{
		private readonly ApplicationState _applicationState;
		private readonly Lazy<DeclarationVisitor> _declarationVisitor;
		private readonly Lazy<ValueDefinitionVisitor> _valueDefinitionVisitor;
		private readonly Lazy<PropertyResolver> _resolver;
		private readonly Lazy<ChoiceVisitor> _choiceVisitor;
		private readonly Stack<FunctionParameters> _result;

		public ParameterParser(ApplicationState applicationState,
							  Lazy<DeclarationVisitor> declarationVisitor,
							  Lazy<ValueDefinitionVisitor> valueDefinitionVisitor,
							  Lazy<PropertyResolver> resolver,
							  Lazy<ChoiceVisitor> choiceVisitor)
		{
			_applicationState = applicationState;
			_declarationVisitor = declarationVisitor;
			_valueDefinitionVisitor = valueDefinitionVisitor;
			_resolver = resolver;
			_choiceVisitor = choiceVisitor;
			_result = new Stack<FunctionParameters>();
		}

		public FunctionParameters GetDefinitionParameters(Verse.Param_def_itemContext function_paramContext)
		{
			_result.Push(new FunctionParameters());
			ParseParametersRecursivly(function_paramContext);
			return _result.Pop();
		}

		public FunctionParameters GetCallParameters(Verse.Param_call_itemContext function_paramContext)
		{
			_result.Push(new FunctionParameters());
			ParseCallParametersRecursively(function_paramContext);
			return _result.Pop();
		}

		private void ParseParametersRecursivly(Verse.Param_def_itemContext function_paramContext)
		{
			if (function_paramContext == null)
			{
				return;
			}
			var declaration = function_paramContext.declaration().Accept(_declarationVisitor.Value);
			_result.Peek().Parameters.Add(declaration);
			var nextChild = function_paramContext.param_def_item();
			ParseParametersRecursivly(nextChild);
		}

		private void ParseCallParametersRecursively(Verse.Param_call_itemContext call_paramContext)
		{
			if (call_paramContext == null)
			{
				return;
			}

			Variable variable = null!;
			var identifier = call_paramContext.ID();
			var result = call_paramContext.value_definition();
			var choice = call_paramContext.choice_rule();

			if (identifier != null)
			{
				variable = _resolver.Value.ResolveProperty(identifier.GetText());
			}
			if (result != null)
			{
				variable = VariableConverter.Convert(_valueDefinitionVisitor.Value.Visit(result)!);
			}
			if (choice != null)
			{
				variable = VariableConverter.Convert(_choiceVisitor.Value.Visit(choice));
			}

			_result.Peek().Parameters.Add(variable!);
			ParseCallParametersRecursively(call_paramContext.param_call_item());
		}
	}
}