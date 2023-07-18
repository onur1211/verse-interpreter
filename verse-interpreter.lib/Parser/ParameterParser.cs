using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Extensions;
using verse_interpreter.lib.Grammar;
using verse_interpreter.lib.ParseVisitors;

namespace verse_interpreter.lib.Visitors
{
    public class ParameterParser
    {
        private readonly ApplicationState _applicationState;
        private readonly DeclarationVisitor _declarationVisitor;
        private readonly ValueDefinitionVisitor _valueDefinitionVisitor;
        private readonly PropertyResolver _resolver;
        private FunctionParameters _result;

        public ParameterParser(ApplicationState applicationState,
                              DeclarationVisitor declarationVisitor,
                              ValueDefinitionVisitor valueDefinitionVisitor,
                              PropertyResolver resolver)
        {
            _applicationState = applicationState;
            _declarationVisitor = declarationVisitor;
            _valueDefinitionVisitor = valueDefinitionVisitor;
            _resolver = resolver;
            _result = new FunctionParameters();
        }

        public FunctionParameters GetDefinitionParameters(Verse.Param_def_itemContext function_paramContext)
        {
            ParseParametersRecursivly(function_paramContext);
            var parameters = _result;
            _result = new FunctionParameters();
            return parameters;
        }

        public FunctionParameters GetCallParameters(Verse.Param_call_itemContext function_paramContext)
        {
            ParseCallParametersRecursively(function_paramContext);
            var parameters = _result;
            _result = new FunctionParameters();
            return parameters;
        }

        private void ParseParametersRecursivly(Verse.Param_def_itemContext function_paramContext)
        {
            if (function_paramContext == null)
            {
                return;
            }
            var declaration = function_paramContext.declaration().Accept(_declarationVisitor);
            _result.Parameters.Add(declaration);
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

            if (identifier != null)
            {
                variable = _resolver.ResolveProperty(identifier.GetText());
            }
            if (result != null)
            {
                variable = Converter.VariableConverter.Convert(result.Accept(_valueDefinitionVisitor));
            }

            _result.Parameters.Add(variable!);
            ParseCallParametersRecursively(call_paramContext.param_call_item());
        }
    }
}