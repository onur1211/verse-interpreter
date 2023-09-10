using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verse_interpreter.lib.Data;
using verse_interpreter.lib.Data.Interfaces;
using verse_interpreter.lib.Data.ResultObjects.Validators;
using verse_interpreter.lib.Data.Variables.Utility;
using verse_interpreter.lib.Exceptions;
using verse_interpreter.lib.ParseVisitors;

namespace verse_interpreter.lib.Evaluation.FunctionEvaluator
{
    public class FunctionCallPreprocessor
    {
        public ParameterValidator FunctionCallValidator { get; } 

        public FunctionCallPreprocessor(ParameterValidator functionCallValidator)
        {
			FunctionCallValidator = functionCallValidator;
        }

        public bool TryBuildExecutableFunction(FunctionCall item)
        {
            if (!IsArityEqual(item))
            {
                throw new NotEqualArityException($"The arity of the function is {item.Function.ParameterCount}, but only {item.Parameters.ParameterCount} parameters were given!");
            }
            if (!FunctionCallValidator.IsTypeConformityGiven(item))
            {
                return false;
            }
            BindValues(item);
            return true;
        }

        private bool IsArityEqual(FunctionCall item)
        {
            return item.Parameters.ParameterCount == item.Function.ParameterCount;
        }

        private void BindValues(FunctionCall item)
        {
            for (int i = 0; i < item.Function.ParameterCount; i++)
            {
                var variable = new Variable()
                {
                    Name = item.Function.Parameters[i].Name,
                    Value = item.Function.Parameters[i].Value.Copy()
                };
                variable.Value = item.Parameters.Parameters[i].Value;
                item.Function.AddScopedVariable(variable);
            }
        }
    }
}
