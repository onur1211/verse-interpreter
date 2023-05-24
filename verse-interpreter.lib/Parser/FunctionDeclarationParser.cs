using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.ResultObjects;
using verse_interpreter.lib.Extensions;
using verse_interpreter.lib.Grammar;

namespace verse_interpreter.lib.Visitors
{
    public class FunctionParser
    {
        private DeclarationVisitor _declarationVisitor;
        private readonly ValueDefinitionVisitor _valueDefinitionVisitor;
        private FunctionParameters _result;

        public FunctionParser(DeclarationVisitor declarationVisitor,
                              ValueDefinitionVisitor valueDefinitionVisitor)
        {
            _declarationVisitor = declarationVisitor;
            _valueDefinitionVisitor = valueDefinitionVisitor;
            _result = new FunctionParameters();
        }

        public FunctionParameters GetDefintionParameters(Verse.Param_def_itemContext function_paramContext)
        {
            ParseParametersRecursivly(function_paramContext);
            var parameters = _result;
            _result = new FunctionParameters();
            return parameters;
        }

        public FunctionParameters GetCallParamters(Verse.Param_call_itemContext function_paramContext)
        {
            ParseCallParametersRecursivly(function_paramContext);
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

        private void ParseCallParametersRecursivly(Verse.Param_call_itemContext call_paramContext)
        {
            if (call_paramContext == null)
            {
                return; 
            }
            var result = Converter.VariableConverter.Convert(call_paramContext.value_definition().Accept(_valueDefinitionVisitor));
            _result.Parameters.Add(result);
            ParseCallParametersRecursivly(call_paramContext.param_call_item());
        }
    }
}